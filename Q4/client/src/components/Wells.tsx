import { IWell } from '../interfaces';

interface WellProps {
	wells: IWell[]
	setWellId: (wellId: string) => void
}

function Table(props: WellProps) {

	return (
		<div className='stack'>
			<h2>Choose a well</h2>
			<table>
				<thead>
					<tr>
						<th>Area</th>
						<th>Field</th>
						<th>UWI</th>
					</tr>
				</thead>
				<tbody>
					{props.wells.map(well => (
						<tr key={well.id} onClick={() => props.setWellId(well.id)}>
							<td>{well.area}</td>
							<td>{well.field}</td>
							<td>{well.UWI}</td>
						</tr>
					))}
				</tbody>
			</table>
		</div>
	);
}

export default Table;