import { Route, Routes, Link, useNavigate } from 'react-router-dom'
import Login from './pages/Login'
import Register from './pages/Register'
import Apply from './pages/Apply'
import Dashboard from './pages/Dashboard'
import Admin from './pages/Admin'
import { useAuth } from './components/auth'

export default function App(){
  const { user, logout } = useAuth()
  const nav = useNavigate()
  return (
    <>
      <nav>
        <div className="flex">
          <Link to="/">AI LOS</Link>
          <Link to="/apply">Apply</Link>
          {user?.role === 'Admin' && <Link to="/admin">Admin</Link>}
        </div>
        <div className="flex right">
          {!user ? (<>
            <Link to="/login">Login</Link>
            <Link to="/register">Register</Link>
          </>) : (<>
            <span className="badge">{user.email}</span>
            <button onClick={()=>{logout(); nav('/')}}>Logout</button>
          </>)}
        </div>
      </nav>
      <div className="container">
        <Routes>
          <Route path="/" element={<Dashboard/>} />
          <Route path="/login" element={<Login/>} />
          <Route path="/register" element={<Register/>} />
          <Route path="/apply" element={<Apply/>} />
          <Route path="/admin" element={<Admin/>} />
        </Routes>
      </div>
    </>
  )
}
