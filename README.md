pomelo-unityclient
=============================
This is the pomelo client for unity3d. The project is based on some libraries as follows:

* WebSocket4Net  (http://websocket4net.codeplex.com/), and you should choose the .Net 3.5 runtime version.

* UnitySocketIO (https://github.com/NetEase/UnitySocketIO).

## How to use
It is very simple to use pomelo-unityclient. Copy all the DLLS locating in the file of /bin/Debug/ 
to your project.

Of cource, you can download this project and compile it:

>git clone  https://github.com/NetEase/pomelo-unityclient.git

## API

Create and initialize a new pomelo client.

```c#
PomeloClient pclient = new PomeloClient(url);
pclient.init();

```

Send request to server and process data in callback.

```c#
pclient.request(route, message, (data)=>{
    //process the data
});
```
Notify server without response

```c#
pclient.notify(route, messge);
```
Add event listener, process broadcast message
```c#
pclient.On(route, (data)=>{
    //process the data
});
```
Disconnect the client.
```c#
pclient.disconnect();
```
##License
(The MIT License)

Copyright (c) 2012-2013 Netease, Inc. and other contributors

Permission is hereby granted, free of charge, to any person obtaining a 
copy of this software and associated documentation files (the 'Software'), 
to deal in the Software without restriction, including without limitation
the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the 
Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in 
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
