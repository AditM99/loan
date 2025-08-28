import { createContext, useContext, useEffect, useState } from 'react'
import api from '../api/client'

type User = { email: string; name: string; role: string }
type AuthContextType = {
  user: User | null
  token: string | null
  login: (email: string, password: string) => Promise<void>
  register: (name: string, email: string, password: string) => Promise<void>
  logout: () => void
}

const AuthContext = createContext<AuthContextType>(null!)

export function AuthProvider({ children }: { children: React.ReactNode }){
  const [user, setUser] = useState<User | null>(null)
  const [token, setToken] = useState<string | null>(null)

  useEffect(()=>{
    const saved = localStorage.getItem('auth')
    if(saved){
      const { token, user } = JSON.parse(saved)
      setUser(user); setToken(token)
      api.setToken(token)
    }
  }, [])

  async function login(email: string, password: string){
    const res = await api.post('/api/auth/login', { email, password })
    const payload = res.data as { token: string, name: string, email: string, role: string }
    const u = { email: payload.email, name: payload.name, role: payload.role }
    setUser(u); setToken(payload.token); api.setToken(payload.token)
    localStorage.setItem('auth', JSON.stringify({ token: payload.token, user: u }))
  }

  async function register(name: string, email: string, password: string){
    const res = await api.post('/api/auth/register', { name, email, password })
    const payload = res.data as { token: string, name: string, email: string, role: string }
    const u = { email: payload.email, name: payload.name, role: payload.role }
    setUser(u); setToken(payload.token); api.setToken(payload.token)
    localStorage.setItem('auth', JSON.stringify({ token: payload.token, user: u }))
  }

  function logout(){
    setUser(null); setToken(null); api.setToken(null); localStorage.removeItem('auth')
  }

  return <AuthContext.Provider value={{ user, token, login, register, logout }}>{children}</AuthContext.Provider>
}

export function useAuth(){ return useContext(AuthContext) }
