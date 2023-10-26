using Content.Server.Actions;
using Content.Server.Humanoid;
using Content.Server.Inventory;
using Content.Server.Mind.Commands;
using Content.Server.Nutrition;
using Content.Server.Polymorph.Components;
using Content.Shared.Actions;
using Content.Shared.Buckle;
using Content.Shared.Damage;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Mind;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Polymorph;
using Content.Shared.Popups;
using JetBrains.Annotations;
using Robust.Server.Containers;
using Robust.Server.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Resources.MAZOR - mods.components;
using Resources.MAZOR - mods.systems;



namespace Resources.MAZOR - mods.Systems
{
    public sealed partial class ChangeEntetyPrototypeSystem : EntitySystem
    {

        
        public override void Initialize()
        {
            base.Initialize();
            
        
        }

        private void Ate(EntityUid uid, ChangeEntetyPrototypeComponent component, Something args)
        {
            PolymorphEntity(uid, component.Polymorph - prototype);
        }

        /// <summary>
        /// Polymorphs the target entity into the specific polymorph prototype
        /// </summary>
        /// <param name="target">The entity that will be transformed</param>
        /// <param name="id">The id of the polymorph prototype</param>
        public EntityUid? PolymorphEntity(EntityUid target, string id)
        {
            if (!_proto.TryIndex<PolymorphPrototype>(id, out var proto))
            {
                _sawmill.Error("Invalid polymorph prototype {id}");
                return null;
            }

            return PolymorphEntity(target, proto);
        }

        /// <summary>
        /// Polymorphs the target entity into the specific polymorph prototype
        /// </summary>
        /// <param name="uid">The entity that will be transformed</param>
        /// <param name="proto">The polymorph prototype</param>
        public EntityUid? PolymorphEntity(EntityUid uid, PolymorphPrototype proto)
        {
            // if it's already morphed, don't allow it again with this condition active.
            if (!proto.AllowRepeatedMorphs && HasComp<PolymorphedEntityComponent>(uid))
                return null;

            // mostly just for vehicles
            _buckle.TryUnbuckle(uid, uid, true);

            var targetTransformComp = Transform(uid);

            var child = Spawn(proto.Entity, targetTransformComp.Coordinates);
            MakeSentientCommand.MakeSentient(child, EntityManager);

            var comp = _compFact.GetComponent<PolymorphedEntityComponent>();
            comp.Parent = uid;
            comp.Prototype = proto.ID;
            AddComp(child, comp);

            var childXform = Transform(child);
            childXform.LocalRotation = targetTransformComp.LocalRotation;

            if (_container.TryGetContainingContainer(uid, out var cont))
                cont.Insert(child);

            //Transfers all damage from the original to the new one
            if (proto.TransferDamage &&
                TryComp<DamageableComponent>(child, out var damageParent) &&
                _mobThreshold.GetScaledDamage(uid, child, out var damage) &&
                damage != null)
            {
                _damageable.SetDamage(child, damageParent, damage);
            }

            if (proto.Inventory == PolymorphInventoryChange.Transfer)
            {
                _inventory.TransferEntityInventories(uid, child);
                foreach (var hand in _hands.EnumerateHeld(uid))
                {
                    _hands.TryDrop(uid, hand, checkActionBlocker: false);
                    _hands.TryPickupAnyHand(child, hand);
                }
            }
            else if (proto.Inventory == PolymorphInventoryChange.Drop)
            {
                if (_inventory.TryGetContainerSlotEnumerator(uid, out var enumerator))
                {
                    while (enumerator.MoveNext(out var slot))
                    {
                        _inventory.TryUnequip(uid, slot.ID, true, true);
                    }
                }

                foreach (var held in _hands.EnumerateHeld(uid))
                {
                    _hands.TryDrop(uid, held);
                }
            }

            if (proto.TransferName && TryComp<MetaDataComponent>(uid, out var targetMeta))
                _metaData.SetEntityName(child, targetMeta.EntityName);

            if (proto.TransferHumanoidAppearance)
            {
                _humanoid.CloneAppearance(uid, child);
            }

            if (_mindSystem.TryGetMind(uid, out var mindId, out var mind))
                _mindSystem.TransferTo(mindId, child, mind: mind);

            //Ensures a map to banish the entity to
            EnsurePausesdMap();
            if (PausedMap != null)
                _transform.SetParent(uid, targetTransformComp, PausedMap.Value);

            return child;
        }



    }
}
