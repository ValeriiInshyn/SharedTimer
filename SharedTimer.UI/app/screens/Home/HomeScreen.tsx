// app/screens/Home/HomeScreen.tsx
import React from "react";
import { View, Text, Button, StyleSheet } from "react-native";
import { useAuth } from "../../../contexts/AuthContext";

export default function HomeScreen() {
  const { logout } = useAuth();

  return (
    <View style={styles.container}>
      <Text style={styles.text}>Welcome to Main Layout!</Text>
      <Button title="Logout" onPress={logout} />
    </View>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, justifyContent: "center", alignItems: "center" },
  text: { fontSize: 22 },
});
