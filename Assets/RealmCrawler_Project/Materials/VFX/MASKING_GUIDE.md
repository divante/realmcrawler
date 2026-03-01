# Masking Guide for Flowing Energy Shader

## ✅ Changes Made

1. **Shader Updated** - Now reads mask from RGB OR Alpha channels (more versatile)
2. **Material Updated** - Better pan speeds for visible motion (0.3 X, 0.5 Y)
3. **Texture Generator Tool Added** - Generate custom mask textures instantly!

---

## 🎨 How Masking Works Now

The shader now uses **any channel** of the mask texture (R, G, B, or Alpha):
- **White areas** = Fully visible
- **Black areas** = Fully transparent
- **Gray areas** = Partially transparent

This means you can use:
- ✅ Grayscale images
- ✅ Alpha channel masks
- ✅ Any texture with brightness values

---

## 🛠️ Generate Mask Textures (NEW TOOL!)

**To open the tool:**
1. In Unity menu: **RealmCrawler → Tools → Texture Generator**

**Quick Mask Options:**

### **Radial Gradient (Soft Circle)** ⭕
Perfect for:
- Smoke puffs
- Energy orbs
- Glowing effects
- Soft edges

### **Radial Gradient (Hard Edge)** ⚫
Perfect for:
- Defined shapes
- Force fields
- Sharp-edged effects

### **Radial Gradient (Very Soft)** ☁️
Perfect for:
- Wispy smoke
- Fog
- Very subtle effects

### **Vertical Gradient** ↕️
Perfect for:
- Rising smoke
- Waterfalls
- Vertical energy beams

### **Horizontal Gradient** ↔️
Perfect for:
- Wind effects
- Horizontal scrolling
- Curtain-like effects

### **Perlin Noise** 🌫️
Perfect for:
- Cloud-like masks
- Organic shapes
- Natural randomness

---

## 🚀 Quick Setup for Wispy Energy Effect

### Step 1: Generate Textures
1. Open **RealmCrawler → Tools → Texture Generator**
2. Click **"Radial Gradient (Soft Circle)"** - Creates a soft circular mask
3. Click **"Perlin Noise"** - Creates a noise pattern (optional for main texture)

### Step 2: Assign to Material
1. Select your material: `FlowingEnergy_Example`
2. **Main Texture**: 
   - Use `Voronoi 1 - 512x512.png` (you already have this)
   - OR use the generated `PerlinNoise.png`
3. **Mask Texture**: 
   - Use `RadialGradient_Soft.png` (just generated!)

### Step 3: Adjust Settings
- **Tint Color**: Choose your energy color (cyan, purple, green, etc.)
- **Emission Strength**: 3-7 for glow
- **Pan Speed X**: 0.2 to 0.5 (gentle horizontal drift)
- **Pan Speed Y**: 0.5 to 1.0 (upward flow like smoke)

---

## 🎯 Understanding the Layers

```
Main Texture (Panning)     ← Your noise/pattern that scrolls
        ×
Mask Texture (Stationary)  ← Your shape/opacity control
        ×
Color Tint                 ← Your chosen color
        ×
Emission                   ← Glow intensity
        =
Final Wispy Energy Effect! ✨
```

---

## 💡 Pro Tips

### For **Smoke Effect**:
```
Main Texture: Perlin Noise (panning upward)
Mask: Radial Gradient Soft
Pan Speed: (0.1, 0.8) - slow horizontal, fast vertical
Color: Gray (0.7, 0.7, 0.7)
Emission: 1-2
```

### For **Energy Field**:
```
Main Texture: Voronoi (panning in any direction)
Mask: Radial Gradient Soft
Pan Speed: (0.5, 0.2) - swirling motion
Color: Cyan (0.3, 0.8, 1.0)
Emission: 4-6
```

### For **Magic Aura**:
```
Main Texture: Perlin Noise Fine
Mask: Radial Gradient Very Soft
Pan Speed: (0.3, -0.3) - circular flow
Color: Purple (0.8, 0.3, 1.0)
Emission: 5-8
Enable Secondary Layer: Yes
Secondary Pan: (-0.3, 0.3) - opposite direction
```

---

## 🔍 Troubleshooting

**"I don't see any masking!"**
- Make sure Mask Texture is assigned
- Check that the texture has visible contrast (not all white or all black)
- Try generating RadialGradient_Soft using the tool

**"The effect is too solid/not transparent enough"**
- Lower the alpha in Tint Color (0.5-0.8)
- Use a softer mask (RadialGradient_VerySoft)
- Check blend mode (should be SrcBlend=5, DstBlend=1)

**"I can't see the panning animation"**
- Increase Pan Speed values (try 1.0, 1.0)
- Make sure you're in Play Mode or Scene View is animating
- Check that Main Texture has visible patterns

**"The edges are too harsh"**
- Use RadialGradient_Soft or RadialGradient_VerySoft
- In Texture Import Settings, set Filter Mode to Bilinear or Trilinear

---

## 📐 Texture Setup Checklist

When importing your own textures, set these import settings:

**Main Texture (Pattern/Noise):**
- ✅ Texture Type: Default
- ✅ sRGB: ON
- ✅ Wrap Mode: Repeat
- ✅ Filter Mode: Bilinear

**Mask Texture (Shape/Opacity):**
- ✅ Texture Type: Default
- ✅ sRGB: ON
- ✅ Alpha Source: From Input
- ✅ Alpha Is Transparency: ON
- ✅ Wrap Mode: Clamp
- ✅ Filter Mode: Bilinear

---

Now you should see proper masking! Try the Texture Generator tool to create different mask shapes. 🎨
