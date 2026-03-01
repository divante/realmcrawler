# Flowing Energy/Smoke Shader Guide

## 📁 Files Created
- **Shader**: `/Assets/RealmCrawler_Project/Materials/VFX/FlowingEnergy.shader`
- **Example Material**: `/Assets/RealmCrawler_Project/Materials/VFX/FlowingEnergy_Example.mat`

---

## 🎨 What This Shader Does

Creates flowing, wispy energy or smoke effects using:
- ✅ **Main Texture** - Your smoke/cloud/energy pattern
- ✅ **Mask Texture** - Controls the shape and opacity
- ✅ **Panning** - Animated scrolling movement
- ✅ **Optional Secondary Layer** - Adds depth with a second flowing texture
- ✅ **Emission** - Makes it glow
- ✅ **Alpha Blending** - Transparent and additive blending

---

## 🚀 Quick Start

### 1. **Create a Material**
   - Right-click in Project → Create → Material
   - Select shader: `RealmCrawler/VFX/FlowingEnergy`

### 2. **Assign Textures**
   You need at least 2 textures:
   
   **Main Texture** (Required):
   - Use a noise/cloud/smoke texture
   - Example: Perlin noise, cloud patterns, energy swirls
   - RGB channels = visible pattern
   
   **Mask Texture** (Required):
   - Controls the overall shape and opacity
   - Example: Gradient, soft shapes, wispy edges
   - Alpha channel = opacity

### 3. **Adjust Settings**

   **Colors:**
   - `Tint Color` - Change the energy color (blue, green, purple, etc.)
   - `Emission Strength` - How bright/glowing (2-5 for subtle, 5-10 for intense)

   **Panning (Movement):**
   - `Pan Speed X` - Horizontal scroll speed (0.5 = gentle flow)
   - `Pan Speed Y` - Vertical scroll speed (0.2 = slow rise)
   - Use negative values to reverse direction

   **Secondary Layer (Optional):**
   - Check `Use Secondary Layer` to enable
   - Assign a second texture for more complex motion
   - Different pan speeds create depth

---

## 🎯 Common Use Cases

### **Energy Field / Force Field**
```
Color: Cyan (0.3, 0.8, 1.0)
Emission: 4
Pan X: 0.5, Pan Y: 0.2
Blend Mode: Additive (SrcBlend=5, DstBlend=1)
```

### **Smoke / Mist**
```
Color: Gray (0.7, 0.7, 0.7, 0.5)
Emission: 1
Pan X: 0.2, Pan Y: 0.5
Blend Mode: Alpha (SrcBlend=5, DstBlend=10)
```

### **Magic Aura / Spell Effect**
```
Color: Purple (0.8, 0.3, 1.0)
Emission: 6
Pan X: 0.3, Pan Y: -0.4 (swirl effect)
Use Secondary Layer: ON
Secondary Pan: -0.4, 0.3 (opposite direction)
```

### **Fire/Ember Effect**
```
Color: Orange (1.0, 0.5, 0.1)
Emission: 8
Pan X: 0.1, Pan Y: 1.0 (upward)
```

---

## 🎛️ Parameter Reference

### **Textures**
| Property | Description |
|----------|-------------|
| Main Texture | The primary pattern (clouds, noise, energy) |
| Mask Texture | Controls shape and transparency |
| Secondary Texture | Optional second layer for depth |

### **Colors**
| Property | Range | Description |
|----------|-------|-------------|
| Tint Color | RGBA | Overall color and alpha |
| Emission Strength | 0-10 | Glow intensity |

### **Panning**
| Property | Range | Description |
|----------|-------|-------------|
| Pan Speed X | -5 to 5 | Horizontal scroll speed |
| Pan Speed Y | -5 to 5 | Vertical scroll speed |
| Secondary Pan Speed X | -5 to 5 | Second layer horizontal |
| Secondary Pan Speed Y | -5 to 5 | Second layer vertical |
| Secondary Blend | 0-1 | Mix between layers |

### **Rendering**
| Property | Options | Description |
|----------|---------|-------------|
| Src Blend | Various | Source blend mode |
| Dst Blend | Various | Destination blend mode |
| Z Write | Off/On | Write to depth buffer |
| Cull | Off/Front/Back | Face culling |

---

## 🎨 Blend Mode Presets

**Additive (Glowing):**
- Src Blend: 5 (SrcAlpha)
- Dst Blend: 1 (One)

**Alpha Blend (Transparent):**
- Src Blend: 5 (SrcAlpha)
- Dst Blend: 10 (OneMinusSrcAlpha)

**Multiply (Darkening):**
- Src Blend: 2 (DstColor)
- Dst Blend: 0 (Zero)

---

## 💡 Tips & Tricks

1. **No Texture? Use Unity's Built-in Noise**
   - Use Unity's Particle textures from Standard Assets
   - Or create simple noise in Photoshop/GIMP

2. **Vertex Colors Support**
   - The shader respects vertex colors from particle systems
   - Use this to fade effects in/out

3. **Performance**
   - Disable secondary layer when not needed
   - Use smaller textures (512x512 is usually enough)

4. **Combine with Particle Systems**
   - Assign to particles for animated smoke/energy
   - Particles handle spawning, shader handles appearance

5. **Double-Sided Rendering**
   - Set Cull to 0 (Off) to see from both sides
   - Good for planes and quads

---

## 🔧 Where to Use This

- ✅ Magic spell effects
- ✅ Energy shields/barriers
- ✅ Smoke and mist
- ✅ Portal effects
- ✅ Aura around characters/items
- ✅ Environmental atmosphere
- ✅ Projectile trails
- ✅ Particle system materials

---

## 📝 Example Setup

1. Create a **Quad** (Right-click → 3D Object → Quad)
2. Assign your material
3. Rotate to face camera
4. Watch it flow!

For best results with particles:
1. Create **Particle System**
2. Set Renderer → Material to your FlowingEnergy material
3. Adjust Start Color and Start Size
4. The shader will handle the flowing animation automatically

---

## 🎯 Need Textures?

Free texture sources:
- Unity Particle Pack (Asset Store)
- Kenney.nl (Free game assets)
- OpenGameArt.org
- Or generate noise textures in Photoshop/GIMP using:
  - Filter → Render → Clouds
  - Filter → Noise → Perlin Noise

---

Enjoy creating amazing VFX! 🌟
