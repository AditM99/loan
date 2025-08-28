import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import api from '../api/client'
import { useAuth } from '../components/auth'

type AppItem = {
  id:number; loanType:string; amount:number; termMonths:number;
  incomeMonthly:number; debtMonthly:number; creditScore:number;
  status:string; prediction?: { approvalProbability:number, explanation:string }
}

export default function Dashboard(){
  const [apps, setApps] = useState<AppItem[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const { token, user } = useAuth()
  const navigate = useNavigate()

  useEffect(()=>{
    // Check if user is authenticated
    if (!token || !user) {
      console.log('No user logged in, redirecting to login')
      navigate('/login')
      return
    }

    setLoading(true)
    setError(null)
    
    console.log('Dashboard: Current token:', token)
    console.log('Dashboard: Current user:', user)
    
    api.get('/api/applications/user/mine')
      .then(res => {
        console.log('Applications response:', res.data)
        setApps(res.data)
      })
      .catch(err => {
        console.error('Error fetching applications:', err)
        console.error('Error response:', err.response)
        console.error('Error status:', err.response?.status)
        console.error('Error data:', err.response?.data)
        
        // If unauthorized, redirect to login
        if (err.response?.status === 401) {
          console.log('Unauthorized, redirecting to login')
          navigate('/login')
          return
        }
        
        setError(err?.response?.data?.error || 'Failed to load applications')
      })
      .finally(() => setLoading(false))
  }, [token, user, navigate])

  // Show loading while checking authentication
  if (!token || !user) {
    return <div className="card"><h2>Redirecting to login...</h2></div>
  }

  if (loading) return <div className="card"><h2>Loading applications...</h2></div>
  if (error) return <div className="card"><h2>Error: {error}</h2></div>

  return (
    <div className="card">
      <h2>Your Applications ({apps.length})</h2>
      {apps.length === 0 ? (
        <p>No applications found. <a href="/apply">Create your first application</a></p>
      ) : (
        <table>
          <thead><tr>
            <th>ID</th><th>Type</th><th>Amount</th><th>Term</th><th>Status</th><th>AI Probability</th>
          </tr></thead>
          <tbody>
            {apps.map(a=> (
              <tr key={a.id}>
                <td>{a.id}</td>
                <td>{a.loanType}</td>
                <td>${a.amount.toLocaleString()}</td>
                <td>{a.termMonths} mo</td>
                <td><span className="badge">{a.status}</span></td>
                <td>{a.prediction ? (a.prediction.approvalProbability*100).toFixed(1)+'%' : '-'}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  )
}
