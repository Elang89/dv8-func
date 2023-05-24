import React from 'react'

export default function Table(props){

	return(
		<div className='stack'>
			<h2>Choose a well</h2>
			<table>
				<thead>
					<th>Area</th>
					<th>Field</th>
					<th>UWI</th>
				</thead>
				<tbody>
				  {props.data.map(well => (
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
