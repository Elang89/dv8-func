interface IWell {
    id: string,
    area: string,
    field: string,
    UWI: string
    rodStringData: ITaper[]
}

interface ITaper {
    id: string,
    type: string,
    length: number,
    diameter: number
}


export { type IWell, type ITaper };