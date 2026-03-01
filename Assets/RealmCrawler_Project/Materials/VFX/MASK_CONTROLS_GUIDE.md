# Mask Controls Guide - Flowing Energy Shader

## ✅ New Mask Parameters Added!

Your shader now has full control over how the mask works:

---

## 🎛️ Mask Control Parameters

### **Mask Contrast** (0.1 - 5.0)
**What it does:** Controls how sharp or soft the transition is between black and white

```
Value: 0.5   = Very soft, gradual transition
Value: 1.0   = Normal (default)
Value: 2.0   = Sharp, more defined edges ✨ RECOMMENDED
Value: 5.0   = Very sharp, almost hard edges
```

**Power function formula:** `pow(maskValue, contrast)`
- Lower values = softens the mask
- Higher values = sharpens the mask, pushes grays toward black or white

---

### **Mask Min** (0.0 - 1.0) - Black Point
**What it does:** Remaps the darkest value in your mask

```
Value: 0.0   = Pure black stays black (default)
Value: 0.3   = Makes black areas become 30% gray ✨ RECOMMENDED
Value: 0.5   = Removes half the darkness
```

**Use this to:** Cut out dark areas and make them invisible

---

### **Mask Max** (0.0 - 1.0) - White Point
**What it does:** Remaps the brightest value in your mask

```
Value: 1.0   = Pure white stays white (default)
Value: 0.7   = Makes white areas only 70% bright
Value: 0.5   = Reduces brightness by half
```

**Use this to:** Control how bright the bright areas are

---

### **Invert Mask** (Checkbox)
**What it does:** Flips black and white

```
OFF = Black = Transparent, White = Visible (default)
ON  = Black = Visible, White = Transparent
```

**Use this to:** Reverse your mask if you have it backwards

---

## 🎯 How Masking Works Now

The shader processes the mask in this order:

1. **Sample mask texture** → Get grayscale value (0-1)
2. **Remap with Min/Max** → `(value - min) / (max - min)`
3. **Apply Contrast** → `pow(value, contrast)`
4. **Invert (optional)** → `1 - value`
5. **Apply to alpha** → Final transparency

---

## 💡 Common Setups

### **Soft Wispy Smoke**
```
Mask Contrast: 1.0
Mask Min: 0.0
Mask Max: 1.0
Invert: OFF
```
Keeps the full gradient smooth

---

### **Defined Energy Shape** ✨
```
Mask Contrast: 2.0
Mask Min: 0.3
Mask Max: 1.0
Invert: OFF
```
Sharpens edges and cuts out dark areas
**This is what you want for most VFX!**

---

### **Hard-Edged Effect**
```
Mask Contrast: 5.0
Mask Min: 0.4
Mask Max: 0.6
Invert: OFF
```
Creates almost binary black/white masking

---

### **Inverted Glow**
```
Mask Contrast: 1.5
Mask Min: 0.0
Mask Max: 1.0
Invert: ON
```
Dark areas glow, light areas are transparent

---

## 🔍 Understanding the Parameters

### **Think of it like Photoshop Levels:**

```
Mask Min = Input Black Point
Mask Max = Input White Point
Mask Contrast = Gamma/Midpoint adjustment
```

### **Visual Example:**

Original mask: `░░▒▒▓▓██` (gradient from black to white)

**With Min: 0.3, Max: 1.0, Contrast: 2.0:**
1. Remap: Cuts out darkest 30% → `░▒▒▓▓██`
2. Contrast: Sharpens the gradient → `░▒▓███`
3. Result: Clearer, more defined edges!

---

## 🎨 Practical Tips

### **To get crisp edges from a soft gradient:**
1. Increase **Mask Contrast** to 2-3
2. Increase **Mask Min** to 0.2-0.4 (cuts dark areas)
3. Leave **Mask Max** at 1.0

### **To make black = invisible, white = visible:**
1. Make sure **Invert Mask** is OFF
2. Your mask should have:
   - Black = areas you want transparent
   - White = areas you want visible

### **If your effect is too dim:**
1. Lower **Mask Min** (allows more through)
2. Check **Emission Strength** (should be 3-7)

### **If edges are too harsh:**
1. Lower **Mask Contrast** (1.0 or less)
2. Lower **Mask Min** (0.0-0.2)

---

## 🧪 Quick Test

Current material settings:
```
Mask Contrast: 2.0   ← Sharpens the mask
Mask Min: 0.3        ← Cuts out dark 30%
Mask Max: 1.0        ← Keeps white areas full
Invert: OFF          ← Normal mode
```

**What this does:**
- Black areas (0.0-0.3) → Completely invisible
- Mid-gray (0.3-0.7) → Partial transparency with sharp falloff
- White areas (0.7-1.0) → Fully visible and glowing

---

## ✨ Best Practice for VFX

For wispy energy/smoke effects, use:
```
Mask Contrast: 1.5 - 2.5
Mask Min: 0.2 - 0.4
Mask Max: 1.0
```

This gives you:
- ✅ Clean, defined shape
- ✅ Soft but controlled edges
- ✅ No muddy dark areas
- ✅ Good contrast

Adjust from there based on your specific texture!

---

Now you have **full control** over exactly how your mask behaves! 🎉
