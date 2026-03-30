async function fetchDataAsync<T>(
	url: string,
	options?: RequestInit
): Promise<T> {
	const res = await fetch(url, options && options);
	if (!res.ok) throw new Error('Fetch failed');
	return res.json();
}

type Duck = {
	_id: string;
	name: string;
	imgUrl: string;
	quote: string;
	createdAt: string;
	updatedAt: string;
	__v: number;
};

const ducks = await fetchDataAsync<Duck[]>(
	'https://duckpond-89zn.onrender.com/wild-ducks'
);

console.log(ducks);
export {};
