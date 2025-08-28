import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../components/auth'

export default function Login(){
  const { login } = useAuth()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<string | null>(null)
  const nav = useNavigate()

  function extractError(err: any): string {
    const data = err?.response?.data
    if (!data) return 'Login failed'
    if (typeof data === 'string') return data
    if (typeof data?.error === 'string') return data.error
    if (Array.isArray(data?.errors)) return data.errors.join(', ')
    if (typeof data?.title === 'string') return data.title
    try { return JSON.stringify(data) } catch { return 'Login failed' }
  }

  async function submit(e: React.FormEvent){
    e.preventDefault()
    setError(null)
    
    try {
      await login(email.trim(), password)
      nav('/')
    } catch (err: any) {
      const msg = extractError(err)
      setError(msg)
      alert(msg)
    }
  }

  return (
    <div className="card" style={{maxWidth: 420, margin: '40px auto'}}>
      <h2>Login</h2>
      <form onSubmit={submit}>
        <input 
          placeholder="Email" 
          type="email"
          value={email} 
          onChange={e=>setEmail(e.target.value)} 
          required
        />
        <input 
          placeholder="Password" 
          type="password" 
          value={password} 
          onChange={e=>setPassword(e.target.value)} 
          required
        />
        <button type="submit">Login</button>
      </form>
    </div>
  )
}
