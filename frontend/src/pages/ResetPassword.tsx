import { useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import api from "../api/client";
import {
  PasswordValidator,
  isPasswordValid,
} from "../components/PasswordValidator";

export default function ResetPassword() {
  const { token } = useParams();
  const nav = useNavigate();
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [message, setMessage] = useState("");
  const [error, setError] = useState("");

  async function submit(e: React.FormEvent) {
    e.preventDefault();
    setMessage("");
    setError("");

    if (password !== confirmPassword) {
      setError("Passwords do not match.");
      return;
    }
    if (!isPasswordValid(password)) {
      setError("Password does not meet the requirements.");
      return;
    }

    try {
      const res = await api.post("/api/auth/reset-password", {
        token,
        password,
      });
      setMessage(
        res.data.message + " You will be redirected to login shortly."
      );
      setTimeout(() => nav("/login"), 3000);
    } catch (err: any) {
      setError(
        err?.response?.data?.error ||
          "Failed to reset password. The link may be invalid or expired."
      );
    }
  }

  return (
    <div className="card" style={{ maxWidth: 420, margin: "40px auto" }}>
      <h2>Reset Password</h2>
      <form onSubmit={submit}>
        <label htmlFor="password">New Password</label>
        <input
          id="password"
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />
        <PasswordValidator password={password} />

        <label htmlFor="confirmPassword">Confirm New Password</label>
        <input
          id="confirmPassword"
          type="password"
          value={confirmPassword}
          onChange={(e) => setConfirmPassword(e.target.value)}
          required
        />
        <button type="submit">Reset Password</button>
      </form>
      {message && (
        <div style={{ color: "green", marginTop: "1rem" }}>{message}</div>
      )}
      {error && <div style={{ color: "red", marginTop: "1rem" }}>{error}</div>}
    </div>
  );
}
