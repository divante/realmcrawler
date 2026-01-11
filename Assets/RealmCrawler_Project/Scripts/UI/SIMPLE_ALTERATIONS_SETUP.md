# Ultra-Simple Alterations System

## 🎯 How It Works

**ONE pool. ALL modifiers. Pure chaos.**

1. Drag all your SimpleStatModifier assets into one big pool
2. Player rerolls a modifier → picks random one from pool
3. **ANY modifier can become ANY other modifier!**

### Example

**Pool contains:**
- Health +20
- Mana +50
- Fire Damage +5
- Eldritch Damage +10
- Lightning +15%

**Your cloak has:** Health +20  
**You reroll →** Could become Mana +50!  
**You reroll again →** Could become Fire Damage +5!  
**You reroll again →** Could become Health +20 again!

Pure RNG! 🎲

---

## ⚡ Setup (30 Seconds)

### Step 1: Open AlterationsConfig

Select: `Assets/RealmCrawler_Project/ScriptableObjects/Core/AlterationsConfig.asset`

### Step 2: Set Cost

```
Cost:
  Reroll Cost Per Modifier: 50
```

### Step 3: Add ALL Your Modifiers

```
Modifier Pool (Size: 10)  ← Set size to how many modifiers you have
  ├── Element 0: Health_+20.asset
  ├── Element 1: Health_+5%.asset
  ├── Element 2: Mana_+20.asset
  ├── Element 3: Mana_+5%.asset
  ├── Element 4: Fire_Damage+5.asset
  ├── Element 5: Fire_Damage+5%.asset
  ├── Element 6: Lightning_Damage+5.asset
  ├── Element 7: Lightning_Damage+5%.asset
  ├── Element 8: Eldritch_Damage+5.asset
  └── Element 9: Eldritch_Damage+5%.asset
```

**Just drag all your existing SimpleStatModifier assets from:**
`/Assets/RealmCrawler_Project/ScriptableObjects/Character/Stat/Modifier/`

### Step 4: Done!

That's it. Seriously.

---

## 🎮 Testing

1. Open NPC Station
2. Click "Alterations"
3. Select equipment with modifiers
4. Toggle a modifier
5. Click "REROLL SELECTED"
6. Watch your Health become Fire Damage (or whatever!)

---

## 💡 Design Notes

### Why This Is Great

✅ **Simple** - One pool, that's it  
✅ **Flexible** - Total randomness = exciting  
✅ **Balanced by RNG** - Players can get lucky or unlucky  
✅ **Easy to maintain** - Just add new modifiers to pool  
✅ **Replayability** - Never know what you'll get!

### Controlling Probability

Want some modifiers more common? **Add them multiple times!**

```
Modifier Pool:
  - Health +20
  - Health +20  ← Duplicate
  - Health +20  ← Duplicate again
  - Fire Damage +5
  - Mana +50
```

Now Health +20 has 3/5 (60%) chance!

### Progressive System

Start with low-tier modifiers only:
```
Modifier Pool (Early Game):
  - Health +20
  - Mana +20
  - Fire Damage +5
```

Later, add higher-tier modifiers to the same pool:
```
Modifier Pool (Mid Game):
  - Health +20
  - Health +50  ← Added!
  - Mana +20
  - Mana +40   ← Added!
  - Fire Damage +5
  - Fire Damage +10  ← Added!
```

You could even have multiple AlterationsConfig assets:
- `AlterationsConfig_EarlyGame.asset`
- `AlterationsConfig_MidGame.asset`
- `AlterationsConfig_EndGame.asset`

And swap them based on player progression!

---

## 🔧 What You Need To Do

### Current Modifiers You Have:
```
/ScriptableObjects/Character/Stat/Modifier/
  ├── Health_+20.asset
  ├── Health_+5%.asset
  ├── Mana_+20.asset
  ├── Mana_+5%.asset
  ├── Fire_Damage+5.asset
  ├── Fire_Damage+5%.asset
  ├── Lightning_Damage+5.asset
  ├── Lightning_Damage+5%.asset
  ├── Eldritch_Damage+5.asset
  └── Eldritch_Damage+5%.asset
```

### Create More Variety (Optional):

**Higher values:**
- `Health_+50.asset`
- `Health_+100.asset`
- `Mana_+50.asset`
- `Fire_Damage+10.asset`
- etc.

**More percentages:**
- `Health_+10%.asset`
- `Health_+15%.asset`
- `Mana_+10%.asset`
- etc.

**Different stats:**
- `Stamina_+20.asset`
- `Defense_+5.asset`
- `Speed_+10%.asset`
- etc.

Then drag them all into the pool!

---

## 📊 Example Setups

### Balanced (Equal Chances)
```
Pool (10 modifiers):
  Health +20, Health +50, Mana +20, Mana +50,
  Fire +5, Fire +10, Lightning +5, Lightning +10,
  Eldritch +5, Eldritch +10
```
Each has 10% chance.

### Health-Focused (More Health Options)
```
Pool (12 modifiers):
  Health +20, Health +20, Health +50, Health +100,
  Mana +20, Mana +50,
  Fire +5, Fire +10,
  Lightning +5, Lightning +10,
  Eldritch +5, Eldritch +10
```
33% chance for Health, 67% for others.

### High-Risk High-Reward
```
Pool (8 modifiers):
  Health +10, Health +10, Health +10, Health +10,
  Mana +5, Mana +5,
  Fire Damage +100,
  Lightning Damage +100
```
75% chance for garbage, 25% chance for jackpot!

---

## 🚀 That's It!

Literally just:
1. Drag modifiers into pool
2. Reroll picks one randomly
3. Done

Can't get simpler than this! 🎉

