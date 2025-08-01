# ShiftScheduler Application - Complete Deployment Guide

## Prerequisites

1. **Git Repository**: Ensure your code is in a Git repository
2. **Render Account**: Sign up at [render.com](https://render.com)
3. **Google OAuth** (Optional): For Google authentication

## Step 1: Prepare Your Application

### 1.1 Verify Build Success
```bash
dotnet build --configuration Release
```

### 1.2 Test Local Run
```bash
dotnet run --configuration Release
```

## Step 2: Configure Environment Variables

### 2.1 Google OAuth Configuration (Optional)
If you want Google authentication:

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select existing one
3. Enable Google+ API
4. Create OAuth 2.0 credentials
5. Add authorized redirect URIs:
   - `https://your-app-name.onrender.com/signin-google`
   - `https://your-app-name.onrender.com/signin-google/callback`

### 2.2 Update appsettings.Production.json
Replace the Google credentials in `appsettings.Production.json`:
```json
{
  "Authentication": {
    "Google": {
      "ClientId": "your-google-client-id",
      "ClientSecret": "your-google-client-secret"
    }
  }
}
```

## Step 3: Deploy to Render

### 3.1 Connect Your Repository
1. Log in to [Render Dashboard](https://dashboard.render.com)
2. Click "New +" → "Web Service"
3. Connect your Git repository
4. Select the repository containing your ShiftScheduler application

### 3.2 Configure the Web Service
- **Name**: `shiftscheduler-application` (or your preferred name)
- **Environment**: `Dotnet`
- **Build Command**: `dotnet build --no-restore`
- **Start Command**: `dotnet ShiftSchedularApplication.dll`

### 3.3 Environment Variables
Add these environment variables in Render dashboard:

**Required:**
- `ASPNETCORE_ENVIRONMENT`: `Production`
- `ASPNETCORE_URLS`: `http://0.0.0.0:$PORT`
- `PORT`: `80`

**Optional (for Google Auth):**
- `Authentication__Google__ClientId`: Your Google Client ID
- `Authentication__Google__ClientSecret`: Your Google Client Secret

### 3.4 Database Configuration
1. In Render dashboard, go to "Databases"
2. Click "New +" → "PostgreSQL"
3. Name: `shiftscheduler-db`
4. Database: `shiftschedulerdb`
5. User: `shiftscheduler_user`
6. Copy the connection string
7. Add as environment variable: `DATABASE_URL`

## Step 4: Deploy and Monitor

### 4.1 Initial Deployment
1. Click "Create Web Service" in Render
2. Wait for the build to complete (5-10 minutes)
3. Check the logs for any errors

### 4.2 Monitor Deployment
1. **Build Logs**: Check for compilation errors
2. **Runtime Logs**: Monitor application startup
3. **Database Connection**: Verify database connectivity

### 4.3 Common Issues and Solutions

#### Issue: Build Fails
- **Solution**: Check that all packages are properly referenced
- **Verify**: Run `dotnet restore` locally

#### Issue: Database Connection Fails
- **Solution**: Ensure `DATABASE_URL` environment variable is set
- **Check**: Verify PostgreSQL service is running

#### Issue: Application Won't Start
- **Solution**: Check runtime logs in Render dashboard
- **Common Cause**: Missing environment variables

## Step 5: Post-Deployment Verification

### 5.1 Test Application
1. Visit your deployed URL
2. Test user registration/login
3. Test CRUD operations for all entities:
   - Shifts
   - Availabilities
   - PayStubs
   - Punches

### 5.2 Security Verification
1. Ensure HTTPS is enabled
2. Test authentication flows
3. Verify user authorization

## Step 6: Maintenance

### 6.1 Monitoring
- Set up alerts in Render dashboard
- Monitor application logs regularly
- Check database performance

### 6.2 Updates
- Push changes to your Git repository
- Render will automatically redeploy
- Monitor deployment logs for issues

## Troubleshooting

### Build Errors
```bash
# Local testing
dotnet clean
dotnet restore
dotnet build --configuration Release
```

### Runtime Errors
1. Check Render logs
2. Verify environment variables
3. Test database connectivity

### Database Issues
1. Verify PostgreSQL service is running
2. Check connection string format
3. Ensure database user has proper permissions

## Environment Variables Reference

| Variable | Description | Required |
|----------|-------------|----------|
| `ASPNETCORE_ENVIRONMENT` | Environment (Production) | Yes |
| `ASPNETCORE_URLS` | Application URLs | Yes |
| `PORT` | Port number | Yes |
| `DATABASE_URL` | PostgreSQL connection string | Yes |
| `Authentication__Google__ClientId` | Google OAuth Client ID | No |
| `Authentication__Google__ClientSecret` | Google OAuth Client Secret | No |

## Support

If you encounter issues:
1. Check Render documentation
2. Review application logs
3. Test locally with same configuration
4. Contact support with specific error messages

## Security Notes

1. **Never commit secrets** to your repository
2. Use environment variables for sensitive data
3. Enable HTTPS in production
4. Regularly update dependencies
5. Monitor for security vulnerabilities 