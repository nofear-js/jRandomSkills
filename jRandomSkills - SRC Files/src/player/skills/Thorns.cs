using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using src.utils;
using static src.jRandomSkills;

namespace src.player.skills
{
    public class Thorns : ISkill
    {
        private const Skills skillName = Skills.Thorns;

        public static void LoadSkill()
        {
            SkillUtils.RegisterSkill(skillName, SkillsInfo.GetValue<string>(skillName, "color"));
        }

        public static void PlayerHurt(EventPlayerHurt @event)
        {
            var attacker = @event.Attacker;
            var victim = @event.Userid;

            if (!Instance.IsPlayerValid(attacker) || !Instance.IsPlayerValid(victim) || attacker == victim) return;
            var victimInfo = Instance.SkillPlayer.FirstOrDefault(p => p.SteamID == victim?.SteamID);
            if (victimInfo?.Skill == skillName && victim!.PawnIsAlive && attacker!.PawnIsAlive)
            {
                int damageToReflect = (int)(@event.DmgHealth * SkillsInfo.GetValue<float>(skillName, "healthTakenScale"));
                int currentHealth = attacker.PlayerPawn.Value.Health;
                
                int actualDamage = Math.Min(damageToReflect, currentHealth - 1);
                if (actualDamage > 0)
                {
                    SkillUtils.TakeHealth(attacker.PlayerPawn.Value, actualDamage);
                    attacker.EmitSound("Player.DamageBody.Onlooker");
                }
            }
        }

        public class SkillConfig(Skills skill = skillName, bool active = true, string color = "#962631", CsTeam onlyTeam = CsTeam.None, bool disableOnFreezeTime = false, bool needsTeammates = false, string requiredPermission = "", float healthTakenScale = .3f) : SkillsInfo.DefaultSkillInfo(skill, active, color, onlyTeam, disableOnFreezeTime, needsTeammates, requiredPermission)
        {
            public float HealthTakenScale { get; set; } = healthTakenScale;
        }
    }
}