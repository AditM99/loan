import axios from 'axios'

const instance = axios.create({
  baseURL: 'http://localhost:5000',
})

let authToken: string | null = null

instance.interceptors.request.use((config)=>{
  console.log('API Request:', config.method?.toUpperCase(), config.url)
  console.log('Current auth token:', authToken)
  
  if(authToken){
    config.headers = config.headers || {}
    config.headers['Authorization'] = `Bearer ${authToken}`
    console.log('Added Authorization header:', `Bearer ${authToken.substring(0, 20)}...`)
  } else {
    console.log('No auth token available')
  }
  return config
})

instance.interceptors.response.use(
  (response) => {
    console.log('API Response:', response.status, response.config.url)
    return response
  },
  (error) => {
    console.log('API Error:', error.response?.status, error.config?.url, error.response?.data)
    return Promise.reject(error)
  }
)

export default {
  get: instance.get,
  post: instance.post,
  setToken: (token: string | null) => { 
    console.log('Setting token:', token ? token.substring(0, 20) + '...' : 'null')
    authToken = token 
  }
}
