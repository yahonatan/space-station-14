using System.Linq;
using System.Text.Json.Serialization;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.Localizations;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;
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
using System.Linq;
using System.Text.Json.Serialization;
using Content.Shared.Administration.Logs;
using Content.Shared.Body.Prototypes;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Content.Shared.Nutrition;
using Robust.Shared.Audio;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.Utility;
using Resources.MAZOR - mods.Systems;

namespace space-station-14.Resources.MAZOR - mods
{
    
    [UsedImplicitly]
    public sealed partial class ChangePrototype : ReagentEffect
    {
        [DataField("polymorph prototype", required: true, customTypeSerializer: typeof(PrototypeIdSerializer<PolymorphPrototype>))]
        public string Polymorph-prototype = default!;
        
        

        


        public override void Effect(ReagentEffectArgs args)
        {
            

            var uid = args.SolutionEntity;
            args.EntityManager.TryGetComponent(uid, out ChangeEntetyProtoypeComponent? component)
                Systems.Get<ChangeEntetyPrototypeSystem>().PolymorphEntity(uid, component);
        }
    }
}
