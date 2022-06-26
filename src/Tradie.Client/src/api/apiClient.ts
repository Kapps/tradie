import { Message } from 'google-protobuf';
import { appConfig } from '../app/config';

const baseUri = appConfig.apiBaseUrl;

export const get = async (path: string) => {
  //const bytes = payload.serializeBinary();
  const uri = `${baseUri}/${path}`;
  const resp = await fetch(uri, {
    method: 'GET',
    credentials: 'omit',
    /*body: bytes,
    headers: {
      'Content-Type': 'application/protobuf',
    },*/
  });

  if (resp.status < 200 || resp.status >= 300) {
    throw new Error(`Error calling ${path} -- ${resp.status} ${resp.statusText}`);
  }

  const body = await resp.arrayBuffer();
  return new Uint8Array(body);
};

export const post = async (path: string, payload: Message) => {
  const bytes = payload.serializeBinary();
  const uri = `${baseUri}/${path}`;
  const resp = await fetch(uri, {
    method: 'POST',
    body: bytes,
    credentials: 'omit',
    headers: {
      'Content-Type': 'application/protobuf',
    },
  });

  if (resp.status < 200 || resp.status >= 300) {
    throw new Error(`Error posting to ${path} -- ${resp.status} ${resp.statusText}`);
  }

  const body = await resp.arrayBuffer();
  return new Uint8Array(body);
};
