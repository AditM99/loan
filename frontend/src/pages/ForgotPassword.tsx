import { useState } from "react";
import api from "../api/client";

export default function ForgotPassword() {
  const [email, setEmail] = useState("");
  const [message, setMessage] = useState("");
  const [error, setError] = useState("");

  async function submit(e: React.FormEvent) {
    e.preventDefault();
    setMessage("");
    setError("");
    try {
      const res = await api.post("/api/auth/forgot-password", { email });
      setMessage(res.data.message);
    } catch (err: any) {
      setError("An error occurred. Please try again.");
    }
  }

  return (
    <div className="card" style={{ maxWidth: 420, margin: "40px auto" }}>
      <h2>Forgot Password</h2>
      <p>
        Enter your email address and we will send you a link to reset your
        password.
      </p>
      <form onSubmit={submit}>
        <label htmlFor="email">Email</label>
        <input
          id="email"
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        />
        <button type="submit">Send Reset Link</button>
      </form>
      {message && (
        <div style={{ color: "green", marginTop: "1rem" }}>{message}</div>
      )}
      {error && <div style={{ color: "red", marginTop: "1rem" }}>{error}</div>}
    </div>
  );
}
