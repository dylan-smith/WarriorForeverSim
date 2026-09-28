using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WarriorForeverSim.Tests.AbilityTests
{
    [TestClass]
    public class AutoAttackTests
    {
        [TestInitialize]
        public void TestInitialize() => InjectZeroMocks();

        [TestCleanup]
        public void TestCleanup()
        {
            BaseStatCalculator.ClearMocks();
            RandomGenerator.ClearMock();
        }

        [TestMethod]
        public void AutoAttack()
        {
            var state = new SimulationState
            {
                CurrentTime = 7.2
            };
            state.Config.Gear.MainHand = new GearItem
            {
                Speed = 2.6
            };

            Assert.IsTrue(WarriorForeverSim.AutoAttack.CanUse(state));

            WarriorForeverSim.AutoAttack.Use(state);

            Assert.AreEqual(1, state.Events.Count);
            Assert.AreEqual(7.2, state.Events[0].Timestamp, 0.001);
            Assert.AreEqual(typeof(AutoAttackSwingEvent), state.Events[0].GetType());
        }

        [TestMethod]
        public void AutoAttackCantUseWhileSwingTimerOnCooldown()
        {
            var state = new SimulationState();
            state.Config.Gear.MainHand = new GearItem
            {
                Speed = 2.6
            };

            state.Auras.Add(Aura.SwingTimerCooldown);

            Assert.IsFalse(WarriorForeverSim.AutoAttack.CanUse(state));
        }

        [TestMethod]
        public void AutoAttackCantUseWithoutMainHand()
        {
            var state = new SimulationState();
            state.Config.Gear.MainHand = null;

            Assert.IsFalse(WarriorForeverSim.AutoAttack.CanUse(state));
        }

        [TestMethod]
        public void AutoAttackSwingEventSchedulesSwingTimer()
        {
            var state = new SimulationState();
            state.Config.Gear.MainHand = new GearItem
            {
                Speed = 2.6,
                WeaponType = WeaponType.OneHandedSword,
                MinDamage = 100,
                MaxDamage = 200,
            };

            var e = new AutoAttackSwingEvent(7.2);

            e.ProcessEvent(state);

            Assert.IsTrue(state.Auras.Contains(Aura.SwingTimerCooldown));
            Assert.AreEqual(2, state.Events.Count);

            var firstEvent = state.Events.OrderBy(x => x.Timestamp).First();
            var secondEvent = state.Events.OrderBy(x => x.Timestamp).Last();

            Assert.AreEqual(typeof(DamageEvent), firstEvent.GetType());
            Assert.AreEqual(typeof(SwingTimerCompletedEvent), secondEvent.GetType());

            Assert.AreEqual(7.2, firstEvent.Timestamp, 0.001); // damage lands immediately
            Assert.AreEqual(9.8, secondEvent.Timestamp, 0.001); // 2.6 sec weapon speed
        }

        [TestMethod]
        public void AutoAttackSwingEventWithHaste()
        {
            var state = new SimulationState();
            state.Config.Gear.MainHand = new GearItem
            {
                Speed = 2.6,
                WeaponType = WeaponType.OneHandedSword,
                MinDamage = 100,
                MaxDamage = 200,
            };

            BaseStatCalculator.InjectMock(typeof(MeleeHasteCalculator), new FakeStatCalculator(1.3));

            var e = new AutoAttackSwingEvent(7.2);

            e.ProcessEvent(state);

            Assert.IsTrue(state.Auras.Contains(Aura.SwingTimerCooldown));
            Assert.AreEqual(2, state.Events.Count);

            var swingTimer = state.Events.OfType<SwingTimerCompletedEvent>().Single();

            Assert.AreEqual(7.2 + (2.6 / 1.3), swingTimer.Timestamp, 0.001);
        }

        [TestMethod]
        public void SwingTimerCompletedEvent()
        {
            var state = new SimulationState();
            state.Auras.Add(Aura.SwingTimerCooldown);

            var e = new SwingTimerCompletedEvent(9.8);

            e.ProcessEvent(state);

            Assert.IsFalse(state.Auras.Contains(Aura.SwingTimerCooldown));
        }

        [TestMethod]
        public void SwingTimerCompletedEventAuraMissing()
        {
            var state = new SimulationState();

            var e = new SwingTimerCompletedEvent(9.8);

            Assert.ThrowsExactly<Exception>(() => e.ProcessEvent(state));
        }

        [TestMethod]
        public void AutoAttackSwingEventHit()
        {
            var state = new SimulationState();
            state.Config.Gear.MainHand = new GearItem
            {
                Speed = 2.6,
                WeaponType = WeaponType.OneHandedSword,
                MinDamage = 100,
                MaxDamage = 200,
            };

            var e = new AutoAttackSwingEvent(7.2);

            e.ProcessEvent(state);

            var dmg = state.Events.OfType<DamageEvent>().Single();

            Assert.AreSame(dmg, e.DamageEvent);
            Assert.AreEqual(7.2, dmg.Timestamp, 0.001);
            Assert.AreEqual(150, dmg.Damage);
            Assert.AreEqual(DamageType.Hit, dmg.DamageType);
            Assert.AreEqual(0.00, dmg.MissChance);
            Assert.AreEqual(0.00, dmg.CritChance);
            Assert.AreEqual(1.0, dmg.HitChance);
        }

        [TestMethod]
        public void AutoAttackSwingEventHitWithAttackPower()
        {
            var state = new SimulationState();
            state.Config.Gear.MainHand = new GearItem
            {
                Speed = 2.6,
                WeaponType = WeaponType.OneHandedSword,
                MinDamage = 100,
                MaxDamage = 200,
            };

            // 1400 AP / 14 = 100 DPS, * 2.6 speed = 260 bonus damage
            BaseStatCalculator.InjectMock(typeof(MeleeAttackPowerCalculator), new FakeStatCalculator(1400));

            var e = new AutoAttackSwingEvent(7.2);

            e.ProcessEvent(state);

            var dmg = state.Events.OfType<DamageEvent>().Single();

            Assert.AreEqual(410, dmg.Damage, 0.001);
            Assert.AreEqual(DamageType.Hit, dmg.DamageType);
        }

        [TestMethod]
        public void AutoAttackSwingEventHitWithDamageMultiplier()
        {
            var state = new SimulationState();
            state.Config.Gear.MainHand = new GearItem
            {
                Speed = 2.6,
                WeaponType = WeaponType.OneHandedSword,
                MinDamage = 100,
                MaxDamage = 200,
            };

            BaseStatCalculator.InjectMock(typeof(DamageMultiplierCalculator), new FakeStatCalculator(1.1));

            var e = new AutoAttackSwingEvent(7.2);

            e.ProcessEvent(state);

            var dmg = state.Events.OfType<DamageEvent>().Single();

            Assert.AreEqual(165, dmg.Damage, 0.001);
            Assert.AreEqual(DamageType.Hit, dmg.DamageType);
        }

        [TestMethod]
        public void AutoAttackSwingEventMiss()
        {
            var state = new SimulationState();
            state.Config.Gear.MainHand = new GearItem
            {
                Speed = 2.6,
                WeaponType = WeaponType.OneHandedSword,
                MinDamage = 100,
                MaxDamage = 200,
            };

            BaseStatCalculator.InjectMock(typeof(MissChanceCalculator), new FakeStatCalculator(0.09));
            RandomGenerator.InjectMock(new FakeRandomGenerator(0.089));

            var e = new AutoAttackSwingEvent(7.2);

            e.ProcessEvent(state);

            var dmg = state.Events.OfType<DamageEvent>().Single();

            Assert.AreEqual(7.2, dmg.Timestamp, 0.001);
            Assert.AreEqual(0, dmg.Damage);
            Assert.AreEqual(DamageType.Miss, dmg.DamageType);
            Assert.AreEqual(0.09, dmg.MissChance, 0.001);
            Assert.AreEqual(0.00, dmg.CritChance, 0.001);
            Assert.AreEqual(0.91, dmg.HitChance, 0.001);

            // A miss still starts the swing timer
            Assert.IsTrue(state.Auras.Contains(Aura.SwingTimerCooldown));
            Assert.AreEqual(1, state.Events.OfType<SwingTimerCompletedEvent>().Count());
        }

        [TestMethod]
        public void AutoAttackSwingEventCrit()
        {
            var state = new SimulationState();
            state.Config.Gear.MainHand = new GearItem
            {
                Speed = 2.6,
                WeaponType = WeaponType.OneHandedSword,
                MinDamage = 100,
                MaxDamage = 200,
            };

            BaseStatCalculator.InjectMock(typeof(MissChanceCalculator), new FakeStatCalculator(0.09));
            BaseStatCalculator.InjectMock(typeof(MeleeCritCalculator), new FakeStatCalculator(0.20));
            RandomGenerator.InjectMock(new FakeRandomGenerator(0.091, 0.19));

            var e = new AutoAttackSwingEvent(7.2);

            e.ProcessEvent(state);

            var dmg = state.Events.OfType<DamageEvent>().Single();

            Assert.AreEqual(7.2, dmg.Timestamp, 0.001);
            Assert.AreEqual(300, dmg.Damage);
            Assert.AreEqual(DamageType.Crit, dmg.DamageType);
            Assert.AreEqual(0.09, dmg.MissChance, 0.001);
            Assert.AreEqual(0.182, dmg.CritChance, 0.0001);
            Assert.AreEqual(0.728, dmg.HitChance, 0.0001);
        }

        [TestMethod]
        public void AutoAttackSwingEventCritWithCritDamageMultiplier()
        {
            var state = new SimulationState();
            state.Config.Gear.MainHand = new GearItem
            {
                Speed = 2.6,
                WeaponType = WeaponType.OneHandedSword,
                MinDamage = 100,
                MaxDamage = 200,
            };

            BaseStatCalculator.InjectMock(typeof(MeleeCritCalculator), new FakeStatCalculator(0.20));
            BaseStatCalculator.InjectMock(typeof(MeleeCritDamageMultiplierCalculator), new FakeStatCalculator(1.03));
            RandomGenerator.InjectMock(new FakeRandomGenerator(0.5, 0.19));

            var e = new AutoAttackSwingEvent(7.2);

            e.ProcessEvent(state);

            var dmg = state.Events.OfType<DamageEvent>().Single();

            Assert.AreEqual(309, dmg.Damage, 0.001);
            Assert.AreEqual(DamageType.Crit, dmg.DamageType);
        }

        [TestMethod]
        public void AutoAttackSwingEventUsesMeleeRollTypes()
        {
            var state = new SimulationState();
            state.Config.Gear.MainHand = new GearItem
            {
                Speed = 2.6,
                WeaponType = WeaponType.OneHandedSword,
                MinDamage = 100,
                MaxDamage = 200,
            };

            BaseStatCalculator.InjectMock(typeof(MissChanceCalculator), new FakeStatCalculator(0.09));
            BaseStatCalculator.InjectMock(typeof(MeleeCritCalculator), new FakeStatCalculator(0.20));

            // Only the melee roll types are scripted; any other roll type would fall back to 0.0 and produce a miss.
            var rng = new FakeRandomGenerator();
            rng.SetRolls(RollType.MeleeMiss, 0.5);
            rng.SetRolls(RollType.MeleeCrit, 0.5);
            RandomGenerator.InjectMock(rng);

            var e = new AutoAttackSwingEvent(7.2);

            e.ProcessEvent(state);

            var dmg = state.Events.OfType<DamageEvent>().Single();

            Assert.AreEqual(150, dmg.Damage);
            Assert.AreEqual(DamageType.Hit, dmg.DamageType);
        }

        private void InjectZeroMocks()
        {
            var zeroMock = new FakeStatCalculator(0.0);

            BaseStatCalculator.InjectMock(typeof(AgilityCalculator), zeroMock);
            BaseStatCalculator.InjectMock(typeof(ArcaneResistanceCalculator), zeroMock);
            BaseStatCalculator.InjectMock(typeof(ArmorCalculator), zeroMock);
            BaseStatCalculator.InjectMock(typeof(RangedBonusDamageCalculator), zeroMock);
            BaseStatCalculator.InjectMock(typeof(DamageMultiplierCalculator), new FakeStatCalculator(1.0));
            BaseStatCalculator.InjectMock(typeof(FireResistanceCalculator), zeroMock);
            BaseStatCalculator.InjectMock(typeof(FrostResistanceCalculator), zeroMock);
            BaseStatCalculator.InjectMock(typeof(HealthCalculator), zeroMock);
            BaseStatCalculator.InjectMock(typeof(IntellectCalculator), zeroMock);
            BaseStatCalculator.InjectMock(typeof(ManaCalculator), zeroMock);
            BaseStatCalculator.InjectMock(typeof(MeleeAttackPowerCalculator), zeroMock);
            BaseStatCalculator.InjectMock(typeof(MeleeCritCalculator), zeroMock);
            BaseStatCalculator.InjectMock(typeof(MeleeCritDamageMultiplierCalculator), new FakeStatCalculator(1.0));
            BaseStatCalculator.InjectMock(typeof(MeleeHasteCalculator), new FakeStatCalculator(1.0));
            BaseStatCalculator.InjectMock(typeof(MissChanceCalculator), zeroMock);
            BaseStatCalculator.InjectMock(typeof(MovementSpeedCalculator), zeroMock);
            BaseStatCalculator.InjectMock(typeof(MP5Calculator), zeroMock);
            BaseStatCalculator.InjectMock(typeof(NatureResistanceCalculator), zeroMock);
            BaseStatCalculator.InjectMock(typeof(RangedAttackPowerCalculator), zeroMock);
            BaseStatCalculator.InjectMock(typeof(RangedCritCalculator), zeroMock);
            BaseStatCalculator.InjectMock(typeof(RangedCritDamageMultiplierCalculator), new FakeStatCalculator(1.0));
            BaseStatCalculator.InjectMock(typeof(RangedHasteCalculator), new FakeStatCalculator(1.0));
            BaseStatCalculator.InjectMock(typeof(ShadowResistanceCalculator), zeroMock);
            BaseStatCalculator.InjectMock(typeof(SpellCritCalculator), zeroMock);
            BaseStatCalculator.InjectMock(typeof(SpiritCalculator), zeroMock);
            BaseStatCalculator.InjectMock(typeof(StaminaCalculator), zeroMock);
            BaseStatCalculator.InjectMock(typeof(StrengthCalculator), zeroMock);
            BaseStatCalculator.InjectMock(typeof(WeaponSkillCalculator), new FakeStatCalculator(300));
        }
    }
}
