import { nanoid } from "nanoid";
import { SyntheticEvent, useRef } from 'react';
import { ITaper, IWell } from "../interfaces";

interface EditProps {
	goBack: () => void,
	well: IWell,
	add: (taper: ITaper) => void,
	delete: (e: SyntheticEvent) => void,

}

function Edit(props: EditProps) {
	const rodStringDataExists = props.well?.rodStringData.length > 0;
	const typeRef = useRef(null);
	const diameterRef = useRef(null);
	const lengthRef = useRef(null);

	function addTaper() {
		const type = (document.getElementById('type') as HTMLInputElement);
		const diameter = (document.getElementById('diameter') as HTMLInputElement);
		const length = (document.getElementById('length') as HTMLInputElement);
		props.add({
			id: nanoid(),
			type: type.value,
			length: length.valueAsNumber,
			diameter: diameter === null ? 0 : diameter.valueAsNumber
		});
		type.value = 'Polished Rod';
		diameter.valueAsNumber = 0;
		length.valueAsNumber = 0;
	}

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
					{props.well?.rodStringData.map((taper: ITaper, index: number) => {
						return (
							<tr key={index} data-index={index}>
								<td>{taper.type}</td>
								<td>{taper.length}</td>
								<td>{taper.diameter}</td>
								<td><button className='delete' onClick={props.delete}>X</button></td>
							</tr>
						)
					}
					)}
				</tbody>
			</table>
			<form>
				<div className='cluster'>
					<select id='type'>
						{props.well?.rodStringData.length === 0 ? <option value='Polished Rod'>Polished Rod</option> : null}
						{props.well?.rodStringData.length === 0 ? <option value='Steel'>Steel</option> : <option value='Steel'>Steel</option>}
						<option value='Fiberglass'>Fiberglass</option>
					</select>
					<input id='length' type='number' placeholder="Length" />
					<input id='diameter' type='number' placeholder="Diameter" />
					<button type='button' onClick={addTaper}>Add</button>
				</div>
			</form>
			<div style={{ display: 'flex', flexWrap: 'wrap' }}>
				<button className='back' onClick={props.goBack}>Back</button>
			</div>
		</div>
	);
}

export default Edit;