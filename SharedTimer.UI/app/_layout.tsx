import { Slot, Redirect, useSegments } from "expo-router";
import { AuthProvider, useAuth } from "../contexts/AuthContext";
import React, { useEffect, useState } from "react";

export default function RootLayout() {
  return (
    <AuthProvider>
      <AuthGate />
    </AuthProvider>
  );
}

function AuthGate() {
  const { isAuthenticated } = useAuth();
  const segments = useSegments();
  const [ready, setReady] = useState(false);

  useEffect(() => {
    if (segments && segments.length >= 0) {
      setReady(true);
    }
  }, [segments]);

  if (!ready) return null;

  const inAuthGroup = segments[0] === "(auth)";

  if (!isAuthenticated && !inAuthGroup) {
    return <Redirect href="/(auth)/login" />;
  }

  if (isAuthenticated && inAuthGroup) {
    return <Redirect href="/(main)/home" />;
  }

  return <Slot />;
}
