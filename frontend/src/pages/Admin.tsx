import { useEffect, useState } from 'react'
import api from '../api/client'

type AppItem = {
  id:number; loanType:string; amount:number; termMonths:number; incomeMonthly:number; debtMonthly:number; creditScore:number; status:string;
  prediction?: { approvalProbability:number, explanation:string }
  user?: { name:string, email:string }
}

export default function Admin(){
  const [apps, setApps] = useState<AppItem[]>([])

  async function load(){
    const res = await api.get('/api/admin/applications')
    setApps(res.data)
  }

  useEffect(()=>{ load() }, [])

  async function setStatus(id:number, status:string){
    await api.post(`/api/admin/applications/${id}/status?status=${encodeURIComponent(status)}`)
    load()
  }

  return (
    <div className="card">
      <h2>Admin – All Applications</h2>
      <table>
        <thead><tr>
          <th>ID</th><th>User</th><th>Type</th><th>Amount</th><th>DTI</th><th>Score</th><th>AI</th><th>Status</th><th>Actions</th>
        </tr></thead>
        <tbody>
          {apps.map(a=>{
            const dti = (a.debtMonthly / Math.max(1,a.incomeMonthly))*100
            return (
              <tr key={a.id}>
                <td>#{a.id}</td>
                <td>{a.user?.email}</td>
                <td>{a.loanType}</td>
                <td>${a.amount.toLocaleString()}</td>
                <td>{dti.toFixed(1)}%</td>
                <td>{a.creditScore}</td>
                <td>{a.prediction ? (a.prediction.approvalProbability*100).toFixed(1)+'%' : '-'}</td>
                <td><span className="badge">{a.status}</span></td>
                <td>
                  <button onClick={()=>setStatus(a.id, 'UnderReview')}>Review</button>{' '}
                  <button onClick={()=>setStatus(a.id, 'Approved')}>Approve</button>{' '}
                  <button onClick={()=>setStatus(a.id, 'Rejected')}>Reject</button>
                </td>
              </tr>
            )
          })}
        </tbody>
      </table>
    </div>
  )
}
