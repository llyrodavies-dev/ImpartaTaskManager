import { BrowserRouter, Routes, Route, Link } from 'react-router-dom'
import { useState, useEffect } from 'react'
import './App.css'
import styles from './App.module.css'

function Home() {
  return (
    <div className="p-8 bg-blue-500 text-white rounded-lg">
      <h1 className="text-3xl font-bold">Home Page</h1>
      <p className="mt-4">✅ Tailwind CSS is working!</p>
    </div>
  )
}

function About() {
  const [data, setData] = useState<string>('')

  useEffect(() => {
    // Test fetch API
    fetch('https://api.github.com/zen')
      .then(res => res.text())
      .then(text => setData(text))
  }, [])

  return (
    <div className={styles.about}>
      <h1>About Page</h1>
      <p>✅ CSS Modules working!</p>
      <p>✅ Fetch API result: {data}</p>
    </div>
  )
}

function App() {
  return (
    <BrowserRouter>
      <nav className="bg-gray-800 p-4">
        <Link to="/" className="text-white mr-4 hover:text-blue-300">Home</Link>
        <Link to="/about" className="text-white hover:text-blue-300">About</Link>
      </nav>
      
      <div className="container mx-auto mt-8">
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/about" element={<About />} />
        </Routes>
      </div>
    </BrowserRouter>
  )
}

export default App