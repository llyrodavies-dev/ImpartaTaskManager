import { useState, useEffect } from 'react';
import styles from '../App.module.css';

export default function About() {
  const [data, setData] = useState<string>('');

  useEffect(() => {
    fetch('https://api.github.com/zen')
      .then(res => res.text())
      .then(text => setData(text));
  }, []);

  return (
    <div className={styles.about}>
      <h1>About Page</h1>
      <p>✅ CSS Modules working!</p>
      <p>✅ Fetch API result: {data}</p>
    </div>
  );
}
