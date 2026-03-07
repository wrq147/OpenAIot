import Foundation
import Network

class IOSTcpClient {
    private var connection: NWConnection?
    private var host: String
    private var port: Int
    private var isConnected = false
    private var receiveQueue = DispatchQueue(label: "com.example.tcpclient.receive")
    private var dataReceivedCallback: ((String) -> Void)?
    private var errorCallback: ((Error) -> Void)?
    private var disconnectCallback: (() -> Void)?
    
    init(host: String, port: Int) {
        self.host = host
        self.port = port
    }
    
    func connect() {
        let endpoint = NWEndpoint.hostPort(host: NWEndpoint.Host(host), port: NWEndpoint.Port(integerLiteral: port))
        connection = NWConnection(to: endpoint, using:.tcp)
        connection?.stateUpdateHandler = self.stateDidChange(to:)
        connection?.start(queue:.main)
    }
    
    func send(data: Data) {
        connection?.send(content: data, completion:.contentProcessed({ error in
            if let error = error {
                self.errorCallback?(error)
            }
        }))
    }
    func disconnect() {
        connection?.cancel()
    }
    
    func setDataReceivedCallback(callback: @escaping (Data) -> Void) {
        dataReceivedCallback = callback
    }
    
    func setErrorCallback(callback: @escaping (Error) -> Void) {
        errorCallback = callback
    }
    
    func setDisconnectCallback(callback: @escaping () -> Void) {
        disconnectCallback = callback
    }
    
    private func stateDidChange(to state: NWConnection.State) {
        switch state {
        case.ready:
            isConnected = true
            startReceiving()
        case.failed(let error):
            isConnected = false
            errorCallback?(error)
        case.cancelled:
            isConnected = false
            disconnectCallback?()
        default:
            break
        }
    }
    
    private func startReceiving() {
        receiveQueue.async {
            self.connection?.receive(minimumIncompleteLength: 1, maximumLength: 65536) { data, context, isComplete, error in
				if let data = data,!data.isEmpty {
                    self.dataReceivedCallback?(data)
                }
                if let error = error {
                    self.errorCallback?(error)
                } else if isComplete == false {
                    self.startReceiving()
                }
            }
        }
    }
}
