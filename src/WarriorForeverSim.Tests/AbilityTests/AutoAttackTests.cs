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
            BaseStatCalculator.InjectMock(typeof(CritCalculator), new FakeStatCalculator(0.20));
            RandomGenerator.InjectMock(new FakeRandomGenerator(0.19));

            var e = new AutoAttackSwingEvent(7.2);

            e.ProcessEvent(state);

            var dmg = state.Events.OfType<DamageEvent>().Single();

            Assert.AreEqual(7.2, dmg.Timestamp, 0.001);
            Assert.AreEqual(300, dmg.Damage);
            Assert.AreEqual(DamageType.Crit, dmg.DamageType);
            Assert.AreEqual(0.09, dmg.MissChance, 0.001);
            Assert.AreEqual(0.20, dmg.CritChance, 0.0001);
            Assert.AreEqual(0.71, dmg.HitChance, 0.0001);
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

            BaseStatCalculator.InjectMock(typeof(CritCalculator), new FakeStatCalculator(0.20));
            BaseStatCalculator.InjectMock(typeof(MeleeCritDamageMultiplierCalculator), new FakeStatCalculator(1.03));
            RandomGenerator.InjectMock(new FakeRandomGenerator(0.19));

            var e = new AutoAttackSwingEvent(7.2);

            e.ProcessEvent(state);

            var dmg = state.Events.OfType<DamageEvent>().Single();

            Assert.AreEqual(309, dmg.Damage, 0.001);
            Assert.AreEqual(DamageType.Crit, dmg.DamageType);
        }

        [TestMethod]
        public void AutoAttackSwingEventUsesMeleeAttackTableRollType()
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
            BaseStatCalculator.InjectMock(typeof(CritCalculator), new FakeStatCalculator(0.20));

            // Only the attack table roll type is scripted; any other roll type would fall back to 0.0 and produce a miss.
            var rng = new FakeRandomGenerator();
            rng.SetRolls(RollType.MeleeAttackTable, 0.5);
            RandomGenerator.InjectMock(rng);

            var e = new AutoAttackSwingEvent(7.2);

            e.ProcessEvent(state);

            var dmg = state.Events.OfType<DamageEvent>().Single();

            Assert.AreEqual(150, dmg.Damage);
            Assert.AreEqual(DamageType.Hit, dmg.DamageType);
        }

        [TestMethod]
        public void AutoAttackSwingEventRecordsSwingDetails()
        {
            var state = new SimulationState();
            state.Config.Gear.MainHand = new GearItem
            {
                Name = "Test Sword",
                Speed = 2.6,
                MinDamage = 100,
                MaxDamage = 200,
            };

            BaseStatCalculator.InjectMock(typeof(MeleeHasteCalculator), new FakeStatCalculator(1.3));

            var e = new AutoAttackSwingEvent(7.2);

            e.ProcessEvent(state);

            Assert.AreEqual("Test Sword", Detail(e, "Swing", "Weapon"));
            Assert.AreEqual("2.60s", Detail(e, "Swing", "Base speed"));
            Assert.AreEqual("×1.3", Detail(e, "Swing", "Haste"));
            Assert.AreEqual("2.00s", Detail(e, "Swing", "Swing speed"));
            Assert.AreEqual("9.20s", Detail(e, "Swing", "Next swing ready at"));
        }

        [TestMethod]
        public void AutoAttackSwingEventRecordsHitDetails()
        {
            var state = new SimulationState();
            state.Config.Gear.MainHand = new GearItem
            {
                Speed = 2.6,
                MinDamage = 100,
                MaxDamage = 200,
            };

            BaseStatCalculator.InjectMock(typeof(MissChanceCalculator), new FakeStatCalculator(0.09));
            BaseStatCalculator.InjectMock(typeof(CritCalculator), new FakeStatCalculator(0.20));
            BaseStatCalculator.InjectMock(typeof(MeleeAttackPowerCalculator), new FakeStatCalculator(1400));
            RandomGenerator.InjectMock(new FakeRandomGenerator(0.5));

            var e = new AutoAttackSwingEvent(7.2);

            e.ProcessEvent(state);

            var dmg = e.DamageEvent;

            Assert.AreEqual(DamageType.Hit, dmg.DamageType);
            Assert.AreEqual(0.5, dmg.AttackRoll);
            Assert.IsFalse(dmg.Details.Any(d => d.Section == "Attack table"));
            Assert.AreEqual("150.0", Detail(dmg, "Damage", "Weapon damage (avg)"));
            Assert.AreEqual("1400", Detail(dmg, "Damage", "Attack power"));
            Assert.AreEqual("260.0", Detail(dmg, "Damage", "Attack power bonus"));
            Assert.AreEqual("410.0", Detail(dmg, "Damage", "Final damage"));
            Assert.IsFalse(dmg.Details.Any(d => d.Label == "Crit multiplier"));
        }

        [TestMethod]
        public void AutoAttackSwingEventRecordsCritDetails()
        {
            var state = new SimulationState();
            state.Config.Gear.MainHand = new GearItem
            {
                Speed = 2.6,
                MinDamage = 100,
                MaxDamage = 200,
            };

            BaseStatCalculator.InjectMock(typeof(CritCalculator), new FakeStatCalculator(0.20));
            BaseStatCalculator.InjectMock(typeof(MeleeCritDamageMultiplierCalculator), new FakeStatCalculator(1.03));
            RandomGenerator.InjectMock(new FakeRandomGenerator(0.19));

            var e = new AutoAttackSwingEvent(7.2);

            e.ProcessEvent(state);

            var dmg = e.DamageEvent;

            Assert.AreEqual(DamageType.Crit, dmg.DamageType);
            Assert.AreEqual(0.19, dmg.AttackRoll);
            Assert.AreEqual("×2.06", Detail(dmg, "Damage", "Crit multiplier"));
            Assert.AreEqual("309.0", Detail(dmg, "Damage", "Final damage"));
        }

        [TestMethod]
        public void AutoAttackSwingEventRecordsMissDetails()
        {
            var state = new SimulationState();
            state.Config.Gear.MainHand = new GearItem
            {
                Speed = 2.6,
                MinDamage = 100,
                MaxDamage = 200,
            };

            BaseStatCalculator.InjectMock(typeof(MissChanceCalculator), new FakeStatCalculator(0.09));
            RandomGenerator.InjectMock(new FakeRandomGenerator(0.089));

            var e = new AutoAttackSwingEvent(7.2);

            e.ProcessEvent(state);

            var dmg = e.DamageEvent;

            Assert.AreEqual(DamageType.Miss, dmg.DamageType);
            Assert.AreEqual(0.089, dmg.AttackRoll);
            Assert.IsFalse(dmg.Details.Any(d => d.Section == "Damage"));
        }

        [TestMethod]
        public void AutoAttackSwingEventCritPushedOffTable()
        {
            var state = new SimulationState();
            state.Config.Gear.MainHand = new GearItem
            {
                Speed = 2.6,
                MinDamage = 100,
                MaxDamage = 200,
            };

            BaseStatCalculator.InjectMock(typeof(MissChanceCalculator), new FakeStatCalculator(0.6));
            BaseStatCalculator.InjectMock(typeof(CritCalculator), new FakeStatCalculator(0.5));
            RandomGenerator.InjectMock(new FakeRandomGenerator(0.99));

            var e = new AutoAttackSwingEvent(7.2);

            e.ProcessEvent(state);

            var dmg = e.DamageEvent;

            // miss takes 60% of the table, leaving only 40% for crit and nothing for hit
            Assert.AreEqual(DamageType.Crit, dmg.DamageType);
            Assert.AreEqual(0.6, dmg.MissChance, 0.0001);
            Assert.AreEqual(0.4, dmg.CritChance, 0.0001);
            Assert.AreEqual(0.0, dmg.HitChance, 0.0001);
        }

        [TestMethod]
        public void AutoAttackSwingEventRollOnMissBoundaryIsMiss()
        {
            var state = new SimulationState();
            state.Config.Gear.MainHand = new GearItem
            {
                Speed = 2.6,
                MinDamage = 100,
                MaxDamage = 200,
            };

            BaseStatCalculator.InjectMock(typeof(MissChanceCalculator), new FakeStatCalculator(0.09));
            BaseStatCalculator.InjectMock(typeof(CritCalculator), new FakeStatCalculator(0.20));
            RandomGenerator.InjectMock(new FakeRandomGenerator(0.09));

            var e = new AutoAttackSwingEvent(7.2);

            e.ProcessEvent(state);

            Assert.AreEqual(DamageType.Miss, e.DamageEvent.DamageType);
        }

        private static string Detail(EventInfo e, string section, string label) => e.Details.Single(d => d.Section == section && d.Label == label).Value;

        private void InjectZeroMocks()
        {
            var zeroMock = new FakeStatCalculator(0.0);

            BaseStatCalculator.InjectMock(typeof(AgilityCalculator), zeroMock);
            BaseStatCalculator.InjectMock(typeof(DamageMultiplierCalculator), new FakeStatCalculator(1.0));
            BaseStatCalculator.InjectMock(typeof(MeleeAttackPowerCalculator), zeroMock);
            BaseStatCalculator.InjectMock(typeof(CritCalculator), zeroMock);
            BaseStatCalculator.InjectMock(typeof(MeleeCritDamageMultiplierCalculator), new FakeStatCalculator(1.0));
            BaseStatCalculator.InjectMock(typeof(MeleeHasteCalculator), new FakeStatCalculator(1.0));
            BaseStatCalculator.InjectMock(typeof(MissChanceCalculator), zeroMock);
            BaseStatCalculator.InjectMock(typeof(StrengthCalculator), zeroMock);
            BaseStatCalculator.InjectMock(typeof(WeaponSkillCalculator), new FakeStatCalculator(300));
        }
    }
}
