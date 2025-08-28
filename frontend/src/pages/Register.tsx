import { useState, useMemo } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../components/auth'
import { PasswordValidator, isPasswordValid } from '../components/PasswordValidator'

export default function Register(){
  const { register } = useAuth()
  const [name, setName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<string | null>(null)
  const nav = useNavigate()

  const passwordOk = useMemo(() => isPasswordValid(password), [password])

  function extractError(err: any): string {
    const data = err?.response?.data
    if (!data) return 'Registration failed'
    if (typeof data === 'string') return data
    if (typeof data?.error === 'string') return data.error
    if (Array.isArray(data?.errors)) return data.errors.join(', ')
    if (typeof data?.title === 'string') return data.title
    try { return JSON.stringify(data) } catch { return 'Registration failed' }
  }

  async function submit(e: React.FormEvent){
    e.preventDefault()
    setError(null)

    const trimmedName = name.trim()
    const trimmedEmail = email.trim()

    if (!trimmedName){
      alert('Name is required')
      return
    }
    if (!trimmedEmail){
      alert('Email is required')
      return
    }
    if (!passwordOk){
      alert('Password must be at least 8 characters and include letters, numbers, and a special character')
      return
    }

    try {
      await register(trimmedName, trimmedEmail, password)
      alert('Your account has been created')
      nav('/')
    } catch (err: any) {
      const msg = extractError(err)
      setError(msg)
      alert(msg)
    }
  }

  return (
    <div className="card" style={{maxWidth: 420, margin: '40px auto'}}>
      <h2>Register</h2>
      <form onSubmit={submit}>
        <input 
          placeholder="Name" 
          value={name} 
          onChange={e=>setName(e.target.value)} 
          required
        />
        <input 
          placeholder="Email" 
          type="email"
          value={email} 
          onChange={e=>setEmail(e.target.value)} 
          required
        />
        <div style={{ position: 'relative' }}>
          <input 
            placeholder="Password" 
            type="password" 
            value={password} 
            onChange={e=>setPassword(e.target.value)} 
            required
          />
          <PasswordValidator password={password} />
        </div>
        <button type="submit">Create account</button>
      </form>
    </div>
  )
}
