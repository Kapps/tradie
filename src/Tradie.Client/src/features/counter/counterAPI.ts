
// A mock function to mimic making an async request for data
export function fetchCount(amount = 1) {
  return new Promise<{ data: number }>((resolve) => setTimeout(() => resolve({ data: amount }), 500));
}

export function fetchCountFaster(amount = 1) {
  return new Promise<{ data: number }>((resolve) => {
    setTimeout(() => resolve({ data: amount }), 100);
  });
}

/*export async function sayHello(name: string) {
  const greeterService = new GreeterClient('http://localhost:5181');
  const request = new HelloRequest();
  request.setName(name);
  const response = await greeterService.sayHello(request, null);
  return response.toObject();
}
*/