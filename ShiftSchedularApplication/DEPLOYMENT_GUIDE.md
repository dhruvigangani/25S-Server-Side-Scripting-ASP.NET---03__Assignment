# 🚀 Deployment Guide - Shift Scheduler Application

## 📋 Prerequisites

- GitHub account with your code repository
- Render account ([render.com](https://render.com))
- Google OAuth credentials (for Google login)
- Facebook OAuth credentials (for Facebook login)

## 🎯 Quick Deployment Steps

### 1. **Prepare Your Repository**
```bash
# Ensure all files are committed
git add .
git commit -m "Ready for deployment"
git push origin main
```

### 2. **Create Render Account**
- Go to [render.com](https://render.com)
- Sign up with your GitHub account
- Authorize Render to access your repositories

### 3. **Deploy on Render**

#### **Option A: Automatic Deployment (Recommended)**
1. Click "New +" → "Web Service"
2. Connect your GitHub repository
3. Render will automatically detect the `render.yaml` configuration
4. Click "Create Web Service"

#### **Option B: Manual Configuration**
1. Click "New +" → "Web Service"
2. Connect your GitHub repository
3. Configure the following settings:
   - **Name:** `shiftscheduler-application`
   - **Environment:** `dotnet`
   - **Build Command:** `dotnet build --no-restore`
   - **Start Command:** `dotnet ShiftSchedularApplication.dll`
   - **Plan:** Free (or paid for production)

### 4. **Configure Environment Variables**

In your Render dashboard, add these environment variables:

#### **Required Variables:**
```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:$PORT
PORT=80
```

#### **OAuth Variables (Set these in Render Dashboard):**
```
GOOGLE_CLIENT_ID=your_google_client_id_here
GOOGLE_CLIENT_SECRET=your_google_client_secret_here
FACEBOOK_APP_ID=your_facebook_app_id_here
FACEBOOK_APP_SECRET=your_facebook_app_secret_here
```

### 5. **Database Setup**
- Render will automatically provision a PostgreSQL database
- The connection string will be injected as `DATABASE_URL`
- No additional configuration needed

## 🔧 OAuth Configuration

### **Google OAuth Setup:**
1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select existing one
3. Enable Google+ API
4. Create OAuth 2.0 credentials
5. Add authorized redirect URI: `https://your-app-name.onrender.com/signin-google`
6. Copy Client ID and Client Secret to Render environment variables

### **Facebook OAuth Setup:**
1. Go to [Facebook Developers](https://developers.facebook.com/)
2. Create a new app
3. Add Facebook Login product
4. Configure OAuth redirect URI: `https://your-app-name.onrender.com/signin-facebook`
5. Copy App ID and App Secret to Render environment variables

## 🌐 Domain Configuration

### **Custom Domain (Optional):**
1. In Render dashboard, go to your service
2. Click "Settings" → "Custom Domains"
3. Add your domain
4. Update OAuth redirect URIs with your custom domain

## 🔍 Troubleshooting

### **Common Issues:**

#### **Build Failures:**
```bash
# Check build logs in Render dashboard
# Common fixes:
- Ensure all dependencies are in .csproj
- Check for syntax errors
- Verify .NET 8.0 compatibility
```

#### **Database Connection Issues:**
```bash
# Check if DATABASE_URL is set correctly
# Verify database is provisioned
# Check connection string format
```

#### **OAuth Not Working:**
```bash
# Verify redirect URIs match exactly
# Check environment variables are set
# Ensure OAuth providers are enabled
```

#### **Application Not Starting:**
```bash
# Check start command: dotnet ShiftSchedularApplication.dll
# Verify PORT environment variable
# Check application logs in Render dashboard
```

### **Logs and Monitoring:**
- View real-time logs in Render dashboard
- Monitor application performance
- Check database connections
- Review OAuth authentication flows

## 🔒 Security Checklist

- [ ] HTTPS is enabled (automatic on Render)
- [ ] Environment variables are set securely
- [ ] OAuth redirect URIs are correct
- [ ] Database connection is encrypted
- [ ] Security headers are enabled
- [ ] CSRF protection is active

## 📊 Performance Optimization

### **Render Free Tier Limits:**
- 750 hours/month
- 512MB RAM
- Shared CPU
- 100GB bandwidth

### **Upgrade for Production:**
- Consider paid plans for better performance
- Enable auto-scaling for high traffic
- Set up monitoring and alerts

## 🚀 Post-Deployment

### **Verify Deployment:**
1. Check application is running: `https://your-app-name.onrender.com`
2. Test login functionality
3. Verify OAuth providers work
4. Test database operations
5. Check security headers

### **Monitor Application:**
- Set up uptime monitoring
- Configure error alerts
- Monitor database performance
- Track user authentication

## 📞 Support

### **Render Support:**
- [Render Documentation](https://render.com/docs)
- [Render Community](https://community.render.com)

### **Application Issues:**
- Check application logs in Render dashboard
- Review this deployment guide
- Open GitHub issues for code problems

## 🎉 Success!

Your Shift Scheduler Application is now deployed and ready for use!

**Default Login Credentials:**
- **Email:** `dario@gc.ca`
- **Password:** `Test123$!`

**Features Available:**
- ✅ User authentication (local + OAuth)
- ✅ Shift management
- ✅ Availability tracking
- ✅ Punch clock functionality
- ✅ Pay stub management
- ✅ Secure HTTPS deployment

---

*For additional help, check the main [README.md](README.md) file.* 