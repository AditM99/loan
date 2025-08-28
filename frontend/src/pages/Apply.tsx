import { useState } from 'react'
import api from '../api/client'

export default function Apply(){
  const [loanType, setLoanType] = useState('Personal')
  const [amount, setAmount] = useState(10000)
  const [term, setTerm] = useState(36)
  const [income, setIncome] = useState(5000)
  const [debt, setDebt] = useState(500)
  const [score, setScore] = useState(700)
  const [appId, setAppId] = useState<number | null>(null)
  const [prob, setProb] = useState<number | null>(null)
  const [explain, setExplain] = useState<string | null>(null)
  const [file, setFile] = useState<File | null>(null)
  const [docUrl, setDocUrl] = useState<string | null>(null)

  async function submit(e: React.FormEvent){
    e.preventDefault()
    const res = await api.post('/api/applications', {
      loanType, amount, termMonths: term, incomeMonthly: income, debtMonthly: debt, creditScore: score
    })
    setAppId(res.data.id)
    const pred = await api.post(`/api/applications/${res.data.id}/predict`)
    setProb(pred.data.approvalProbability)
    setExplain(pred.data.explanation)
  }

  async function uploadDoc(){
    if(!appId || !file) return
    const form = new FormData()
    form.append('file', file)
    const res = await api.post(`/api/applications/${appId}/upload?type=Paystub`, form, {
      headers: { 'Content-Type': 'multipart/form-data' }
    })
    setDocUrl((import.meta.env.VITE_API_URL || 'http://localhost:5000') + res.data.url)
  }

  return (
    <div className="card">
      <h2>Apply for a Loan</h2>
      <form onSubmit={submit}>
        <div className="flex">
          <div style={{flex:1}}>
            <label>Loan Type</label>
            <select value={loanType} onChange={e=>setLoanType(e.target.value)}>
              <option>Personal</option>
              <option>Auto</option>
              <option>Mortgage</option>
            </select>
          </div>
          <div style={{flex:1}}>
            <label>Amount</label>
            <input type="number" value={amount} onChange={e=>setAmount(parseFloat(e.target.value))} />
          </div>
        </div>
        <div className="flex">
          <div style={{flex:1}}>
            <label>Term (months)</label>
            <input type="number" value={term} onChange={e=>setTerm(parseInt(e.target.value))} />
          </div>
          <div style={{flex:1}}>
            <label>Monthly Income</label>
            <input type="number" value={income} onChange={e=>setIncome(parseFloat(e.target.value))} />
          </div>
        </div>
        <div className="flex">
          <div style={{flex:1}}>
            <label>Monthly Debt</label>
            <input type="number" value={debt} onChange={e=>setDebt(parseFloat(e.target.value))} />
          </div>
          <div style={{flex:1}}>
            <label>Credit Score</label>
            <input type="number" value={score} onChange={e=>setScore(parseInt(e.target.value))} />
          </div>
        </div>
        <button type="submit">Submit application</button>
      </form>

      {prob !== null && (
        <div className="card" style={{marginTop:16}}>
          <h3>AI Eligibility</h3>
          <p>Approval probability: <strong>{(prob*100).toFixed(1)}%</strong></p>
          <p>Reasoning: <span className="badge">{explain}</span></p>
        </div>
      )}

      {appId && (
        <div className="card" style={{marginTop:16}}>
          <h3>Upload Paystub (optional)</h3>
          <input type="file" onChange={e=>setFile(e.target.files?.[0] || null)} />
          <button onClick={uploadDoc} type="button">Upload</button>
          {docUrl && <p>Uploaded: <a href={docUrl} target="_blank">View</a></p>}
        </div>
      )}
    </div>
  )
}
