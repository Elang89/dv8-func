import { useState, useEffect, SyntheticEvent } from 'react'
import getData from './services/api';
import RodStringEditor from './components/RodStringEditor';
import './App.css'
import Wells from './components/Wells';
import { IWell, ITaper } from './interfaces';

function App() {
	const [wellId, setWellId] = useState("");
	const [data, setData] = useState<IWell[]>([]);

	useEffect(() => {
		const url = 'data.json';
		const method = 'get';
		const headers = {
			'Content-Type': 'application/json',
			'Accept': 'application/json'
		};

		getData(url, method, headers, setData);

	}, []);

	const selectedWell = data.find((well: IWell) => well.id === wellId);

	function goBack() {
		setWellId("");
	}

	function addRow(taper: ITaper) {
		const item = data.find((w: IWell) => w.id === wellId);

		if (item) {
			item.rodStringData.push(taper);
			setData([...data]);
		}
	}

	function deleteRow(e: SyntheticEvent) {
		const index = (e.target as HTMLElement).closest("tr")?.getAttribute("data-index");
		const item = data.find((w: IWell) => w.id === wellId);

		if (item && index) {
			const val = +index;

			item.rodStringData.splice(val, 1);
			setData([...data]);
		}
	}

	return (
		<div className='cover'>
			<h1>Rod String Editor</h1>
			<div className="centered">
				<div className='center'>
					<div hidden={wellId !== ""}>
						<Wells wells={data} setWellId={setWellId} />
					</div>
					<div hidden={wellId === ""}>
						<RodStringEditor goBack={goBack} well={selectedWell} add={addRow} delete={deleteRow} />
					</div>
				</div>
			</div>
		</div>
	)
}

export default App
