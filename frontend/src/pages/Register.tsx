import { useState, useMemo } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../components/auth'
import { PasswordValidator, isPasswordValid } from '../components/PasswordValidator'

export default function Register(){
  const { register } = useAuth()
  const [firstName, setFirstName] = useState('')
  const [lastName, setLastName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [dateOfBirth, setDateOfBirth] = useState('')
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

    const trimmedFirstName = firstName.trim()
    const trimmedLastName = lastName.trim()
    const trimmedEmail = email.trim()

    if (!trimmedFirstName){
      alert('First name is required')
      return
    }
    if (!trimmedLastName){
      alert('Last name is required')
      return
    }
    if (!trimmedEmail){
      alert('Email is required')
      return
    }
    if (!dateOfBirth){
      alert('Date of birth is required')
      return
    }
    if (!passwordOk){
      alert('Password must be at least 8 characters and include letters, numbers, and a special character')
      return
    }

    try {
      await register(trimmedFirstName, trimmedLastName, trimmedEmail, dateOfBirth, password)
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
        <div style={{ display: 'flex', gap: '12px' }}>
          <div style={{ flex: 1 }}>
            <label htmlFor="firstName">
              First Name <span style={{ color: 'red' }}>*</span>
            </label>
            <input 
              id="firstName"
              placeholder="Enter first name" 
              value={firstName} 
              onChange={e=>setFirstName(e.target.value)} 
              required
            />
          </div>
          <div style={{ flex: 1 }}>
            <label htmlFor="lastName">
              Last Name <span style={{ color: 'red' }}>*</span>
            </label>
            <input 
              id="lastName"
              placeholder="Enter last name" 
              value={lastName} 
              onChange={e=>setLastName(e.target.value)} 
              required
            />
          </div>
        </div>

        <label htmlFor="email">
          Email Address <span style={{ color: 'red' }}>*</span>
        </label>
        <input 
          id="email"
          placeholder="Enter email address" 
          type="email"
          value={email} 
          onChange={e=>setEmail(e.target.value)} 
          required
        />

        <label htmlFor="dateOfBirth">
          Date of Birth <span style={{ color: 'red' }}>*</span>
        </label>
        <input 
          id="dateOfBirth"
          type="date" 
          value={dateOfBirth} 
          onChange={e=>setDateOfBirth(e.target.value)} 
          required
        />

        <label htmlFor="password">
          Password <span style={{ color: 'red' }}>*</span>
        </label>
        <div style={{ position: 'relative' }}>
          <input 
            id="password"
            placeholder="Enter password" 
            type="password" 
            value={password} 
            onChange={e=>setPassword(e.target.value)} 
            required
          />
          <PasswordValidator password={password} />
        </div>

        {error && <div className="badge" style={{ color: '#ef4444', backgroundColor: '#fef2f2', border: '1px solid #fecaca' }}>{error}</div>}
        <button type="submit">Create account</button>
      </form>
    </div>
  )
}
