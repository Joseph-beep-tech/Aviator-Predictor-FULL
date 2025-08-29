# 🚀 Quick Start Guide - Aviator Predictor

## ⚡ Get Started in 5 Minutes

### 1. 📥 Download & Extract
- Download the repository ZIP file
- Extract to a folder on your computer
- Navigate to the `Aviator Predictor` folder

### 2. 🛠️ Build the Project

#### Option A: Using Visual Studio (Recommended)
1. Open `Aviator-Hack.sln` in Visual Studio 2022
2. Wait for NuGet packages to restore
3. Press `Ctrl + Shift + B` to build
4. Press `F5` to run

#### Option B: Using Build Script
1. Double-click `build.bat` in the project folder
2. Follow the prompts to install build tools if needed

#### Option C: Using .NET CLI
1. Install .NET 6.0 SDK from [Microsoft](https://dotnet.microsoft.com/download/dotnet/6.0)
2. Open command prompt in project folder
3. Run: `dotnet build --configuration Release`
4. Run: `dotnet run`

### 3. 🔐 Login
- **Username**: `admin`
- **Password**: `admin123`
- Or use demo account: `demo` / `demo123`

### 4. 🎯 Make Your First Prediction
1. Enter a game seed in the "LAST GAME SEED" field
2. Click "NEXT" to validate
3. Click "START" to generate prediction
4. View detailed analysis and confidence score

## 🔧 Troubleshooting

### Build Issues
- **Error**: "No build tools found"
  - **Solution**: Install .NET 6.0 SDK or Visual Studio

- **Error**: "NuGet packages not found"
  - **Solution**: Right-click solution → Restore NuGet Packages

### Runtime Issues
- **Error**: "Login failed"
  - **Solution**: Use correct credentials: admin/admin123

- **Error**: "Invalid game seed"
  - **Solution**: Ensure 64-character hexadecimal format

## 📱 System Requirements

- **OS**: Windows 10/11 (64-bit)
- **RAM**: 4GB minimum, 8GB recommended
- **Storage**: 100MB free space
- **Runtime**: .NET 6.0 or later

## 🎮 Game Seed Format

Game seeds must be exactly 64 hexadecimal characters:
```
Example: a1b2c3d4e5f6789012345678901234567890abcdef1234567890abcdef1234567890
```

## 📊 Understanding Predictions

- **Multiplier**: The predicted game multiplier
- **Confidence**: Accuracy percentage based on historical data
- **Analysis**: Detailed breakdown of prediction factors
- **Trend**: Whether prediction is above/below recent average

## 🆘 Need Help?

1. Check the console output for error messages
2. Verify your game seed format
3. Ensure .NET runtime is properly installed
4. Try the demo account first

## 🎯 Pro Tips

- Start with demo account to learn the interface
- Use real game seeds for actual predictions
- Monitor confidence scores for best results
- Keep the application updated for latest features

---

**Ready to predict? Launch the application and start analyzing Aviator games! 🚁**
