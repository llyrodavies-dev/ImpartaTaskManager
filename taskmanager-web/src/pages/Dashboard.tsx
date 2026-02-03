import { useEffect, useRef, useState } from 'react';
import { api } from '../services/ApiService';

export default function Dashboard() {
    const [profileImageUrl, setProfileImageUrl] = useState<string | null>(null);
    const fileInputRef = useRef<HTMLInputElement>(null);

    useEffect(() => {
        const fetchProfileImage = async () => {
            try {
                const blob = await api.get('Users/profile-image', { responseType: 'blob' });
                const url = URL.createObjectURL(blob);
                setProfileImageUrl(url);
            } catch (err) {
                setProfileImageUrl(null);
            }
        };
        fetchProfileImage();

        return () => {
            if (profileImageUrl) URL.revokeObjectURL(profileImageUrl);
        };
    }, []);

    const handleIconClick = () => {
        fileInputRef.current?.click();
    };

    const handleFileChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (!file) return;
        const formData = new FormData();
        formData.append('file', file);

        try {
            await api.post('Users/profile-image', formData);
            // Refresh the image
            const blob = await api.get('Users/profile-image', { responseType: 'blob' });
            const url = URL.createObjectURL(blob);
            setProfileImageUrl(url);
        } catch (err) {
            setProfileImageUrl(null);
        }
    };

    return (
        <div className="p-8">
            <h1 className="text-2xl font-bold mb-4">Dashboard</h1>
            <div style={{ position: 'relative', display: 'inline-block' }}>
              {profileImageUrl ? (
                  <img
                      src={profileImageUrl}
                      alt="Profile"
                      style={{
                          width: 200,
                          height: 200,
                          objectFit: 'cover',
                          borderRadius: '50%',
                          border: '2px solid #ddd',
                          boxShadow: '0 2px 8px rgba(0,0,0,0.1)',
                      }}
                  />
              ) : (
                  <div className="w-[200px] h-[200px] rounded-full bg-gray-200 flex items-center justify-center text-gray-500">
                      No Image
                  </div>
              )}
              <button
                    type="button"
                    onClick={handleIconClick}
                    style={{
                        position: 'absolute',
                        bottom: 16,
                        right: 16,
                        background: '#fff',
                        borderRadius: '50%',
                        border: '1px solid #ccc',
                        padding: 8,
                        cursor: 'pointer',
                        boxShadow: '0 1px 4px rgba(0,0,0,0.1)',
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                    }}
                    title="Change profile image">
                  
                    <svg width="20" height="20" fill="none" stroke="currentColor" strokeWidth="2" viewBox="0 0 24 24">
                        <path d="M16.5 3.5a2.121 2.121 0 1 1 3 3L7 19l-4 1 1-4 12.5-12.5z" strokeLinecap="round" strokeLinejoin="round" />
                    </svg>
                </button>

                <input
                    ref={fileInputRef}
                    type="file"
                    accept="image/*"
                    style={{ display: 'none' }}
                    onChange={handleFileChange}/>
             </div>
        </div>
    );
}
