import React, { useState } from 'react';
import { useAuth } from '../hooks/useAuth';
import { 
  Button, 
  Input, 
  YStack, 
  XStack, 
  Text, 
  Card, 
  CardHeader, 
  CardContent,
  Separator,
  Avatar,
  Switch,
} from '../components/ui';
import { 
  User, 
  Mail, 
  Bell, 
  Moon, 
  Sun,
  Save,
  Edit,
  X,
} from '../components/icons';

const ProfilePage: React.FC = () => {
  const { user } = useAuth();
  const [isEditing, setIsEditing] = useState(false);
  const [isDarkMode, setIsDarkMode] = useState(false);
  const [notifications, setNotifications] = useState(true);
  const [emailNotifications, setEmailNotifications] = useState(true);
  
  const [profileData, setProfileData] = useState({
    name: user?.name || '',
    email: user?.email || '',
    avatar: user?.avatar || '',
  });

  const handleSave = async () => {
    try {
      // TODO: Implement profile update API call
      console.log('Saving profile:', profileData);
      setIsEditing(false);
    } catch (err) {
      console.error('Failed to update profile:', err);
    }
  };

  const handleAvatarChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      // TODO: Implement file upload
      const reader = new FileReader();
      reader.onload = (e) => {
        setProfileData(prev => ({ ...prev, avatar: e.target?.result as string }));
      };
      reader.readAsDataURL(file);
    }
  };

  return (
    <YStack gap="$6">
      {/* Header */}
      <XStack justifyContent="space-between" alignItems="center">
        <YStack>
          <Text fontSize="$8" fontWeight="bold">Profile</Text>
          <Text color="$gray11">Manage your account settings</Text>
        </YStack>
        
        {isEditing ? (
          <XStack gap="$2">
            <Button
              variant="outline"
              icon={X}
              onPress={() => {
                setIsEditing(false);
                setProfileData({
                  name: user?.name || '',
                  email: user?.email || '',
                  avatar: user?.avatar || '',
                });
              }}
            >
              Cancel
            </Button>
            <Button
              icon={Save}
              onPress={handleSave}
            >
              Save
            </Button>
          </XStack>
        ) : (
          <Button
            variant="outline"
            icon={Edit}
            onPress={() => setIsEditing(true)}
          >
            Edit Profile
          </Button>
        )}
      </XStack>

      {/* Profile Information */}
      <Card>
        <CardHeader>
          <Text fontSize="$6" fontWeight="bold">Personal Information</Text>
        </CardHeader>
        <CardContent padding="$6">
          <YStack gap="$6">
            {/* Avatar */}
            <YStack alignItems="center" gap="$3">
              <Avatar circular size="large" src={profileData.avatar}>
                {profileData.name?.charAt(0).toUpperCase()}
              </Avatar>
              
              {isEditing && (
                <Button
                  variant="outline"
                  size="small"
                  onPress={() => document.getElementById('avatar-input')?.click()}
                >
                  Change Avatar
                </Button>
              )}
              
              <input
                id="avatar-input"
                type="file"
                accept="image/*"
                onChange={handleAvatarChange}
                style={{ display: 'none' }}
              />
            </YStack>

            {/* Name */}
            <YStack gap="$2">
              <Text fontSize="$3" color="$gray11" fontWeight="bold">Name</Text>
              {isEditing ? (
                <Input
                  value={profileData.name}
                  onChangeText={(value) => setProfileData(prev => ({ ...prev, name: value }))}
                  placeholder="Enter your name"
                  icon={User}
                />
              ) : (
                <Text fontSize="$4">{profileData.name}</Text>
              )}
            </YStack>

            {/* Email */}
            <YStack gap="$2">
              <Text fontSize="$3" color="$gray11" fontWeight="bold">Email</Text>
              {isEditing ? (
                <Input
                  value={profileData.email}
                  onChangeText={(value) => setProfileData(prev => ({ ...prev, email: value }))}
                  placeholder="Enter your email"
                  icon={Mail}
                  type="email"
                />
              ) : (
                <Text fontSize="$4">{profileData.email}</Text>
              )}
            </YStack>
          </YStack>
        </CardContent>
      </Card>

      {/* Preferences */}
      <Card>
        <CardHeader>
          <Text fontSize="$6" fontWeight="bold">Preferences</Text>
        </CardHeader>
        <CardContent padding="$6">
          <YStack gap="$4">
            {/* Dark Mode */}
            <XStack justifyContent="space-between" alignItems="center">
              <XStack alignItems="center" gap="$3">
                {isDarkMode ? <Moon size={20} /> : <Sun size={20} />}
                <YStack>
                  <Text fontSize="$4" fontWeight="bold">Dark Mode</Text>
                  <Text fontSize="$3" color="$gray11">Switch between light and dark themes</Text>
                </YStack>
              </XStack>
              <Switch
                checked={isDarkMode}
                onCheckedChange={setIsDarkMode}
              />
            </XStack>

            <Separator />

            {/* Push Notifications */}
            <XStack justifyContent="space-between" alignItems="center">
              <XStack alignItems="center" gap="$3">
                <Bell size={20} />
                <YStack>
                  <Text fontSize="$4" fontWeight="bold">Push Notifications</Text>
                  <Text fontSize="$3" color="$gray11">Receive notifications for task updates</Text>
                </YStack>
              </XStack>
              <Switch
                checked={notifications}
                onCheckedChange={setNotifications}
              />
            </XStack>

            <Separator />

            {/* Email Notifications */}
            <XStack justifyContent="space-between" alignItems="center">
              <XStack alignItems="center" gap="$3">
                <Mail size={20} />
                <YStack>
                  <Text fontSize="$4" fontWeight="bold">Email Notifications</Text>
                  <Text fontSize="$3" color="$gray11">Receive email updates for important events</Text>
                </YStack>
              </XStack>
              <Switch
                checked={emailNotifications}
                onCheckedChange={setEmailNotifications}
              />
            </XStack>
          </YStack>
        </CardContent>
      </Card>

      {/* Account Actions */}
      <Card>
        <CardHeader>
          <Text fontSize="$6" fontWeight="bold">Account Actions</Text>
        </CardHeader>
        <CardContent padding="$6">
          <YStack gap="$3">
            <Button
              variant="outline"
              onPress={() => {
                // TODO: Implement change password functionality
                console.log('Change password');
              }}
            >
              Change Password
            </Button>
            
            <Button
              variant="outline"
              onPress={() => {
                // TODO: Implement export data functionality
                console.log('Export data');
              }}
            >
              Export My Data
            </Button>
            
            <Button
              variant="outline"
              onPress={() => {
                if (confirm('Are you sure you want to delete your account? This action cannot be undone.')) {
                  // TODO: Implement delete account functionality
                  console.log('Delete account');
                }
              }}
            >
              Delete Account
            </Button>
          </YStack>
        </CardContent>
      </Card>
    </YStack>
  );
};

export default ProfilePage; 