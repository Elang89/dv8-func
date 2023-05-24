import { nanoid } from "nanoid";
import { useRef } from 'react';
export default function Edit(props){
	const rodStringDataExists = props.well?.rodStringData.length > 0;
	const typeRef = useRef(null);
	const diameterRef = useRef(null);
	const lengthRef = useRef(null);


	return (
		<div className='stack'>
			<h2>{props.well?.UWI}</h2>
			<table>
				<thead>
					<tr>
						<th>Type</th>
						<th>Length</th>
						<th>Diameter</th>
						<th></th>
					</tr>
				</thead>
				<tbody>
					{props.well?.rodStringData.map((taper,index) => {
						return (
							<tr>
								<td>{taper.type}</td>
								<td>{taper.length}</td>
								<td>{taper.diameter}</td>
								<td><button className='delete' onClick={props.delete}>X</button></td>
							</tr>
						)}
					)}
				</tbody>
			</table>
			<form>
				<div className='cluster'>
					<select id='type'>
						<option value='Polished Rod'>Polished Rod</option>
						<option value='Steel'>Steel</option>
						<option value='Fiberglass'>Fiberglass</option>
					</select>
					<input id='length' type='number' placeholder="Length" />
					<input id='diameter' type='number' placeholder="Diameter" />
					<button type='button'onClick={() => {						
						const type = document.getElementById('type');
						const diameter = document.getElementById('diameter');
						const length = document.getElementById('length');
						props.add({
							id: nanoid(),
							type: type.value,
							length: length.value,
							diameter: diameter.value
						});
						type.value = 'Polished Rod';
						diameter.value = 0;
						length.value = 0;
					}}>Add</button>
				</div>
			</form>
			<div style={{display:'flex', flexWrap: 'wrap'}}>
				<button className='back'onClick={props.goBack}>Back</button>	
			</div>
		</div>
	);
}
