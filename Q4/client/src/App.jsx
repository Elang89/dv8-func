import { useState, useEffect } from 'react'
import RodStringEditor from './components/RodStringEditor';
import './App.css'
import Wells from './components/Wells';

function App() {
	const [wellId, setWellId] = useState(undefined);
	const [data, setData] = useState([]);
	useEffect(() => {
		fetch('data.json', {
			method: 'get',
			headers: {
				'Content-Type': 'application/json',
				'Accept': 'application/json'
			}
		}).then(response => {
			if (!response.ok) {
				throw new Error("HTTP Error: " + response.status);
			}
			return response.json();
		}).then(json => {
			setData(json);
		})
	}, []);
	const selectedWell = data.find(well => well.id === wellId);
	return (
		<div className='cover'>
			<h1>Rod String Editor</h1>
			<div className="centered">
				<div className='center'>
					<div hidden={wellId !== undefined}>
						<Wells data={data} setWellId={setWellId} />
					</div>
					<div hidden={wellId === undefined}>
						<RodStringEditor goBack={() => setWellId(undefined)} well={selectedWell}
							add={(taper) => {
								const prevData = data;

								prevData.find(w => w.id === wellId).rodStringData.push(taper);
								setData(prevData);
							}}
							delete={() => {
								alert("Delete needs to be implemented still");
							}}
						/>
					</div>
				</div>
			</div>
		</div>
	)
}

export default App
