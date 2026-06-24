import mqtt from "mqtt";
import {
    getClientId,
    getWebIp
} from "@/api/system/mqtt";
const mqttclient = {
    namespaced: true,
    state: {
        mqclient: null,
        msgHandlers: {}
    },
    mutations: {
        CHANGE_CLIENT: (state, client) => {
            state.mqclient = client;
        },
        Add_Handler: (state, {
            key,
            func
        }) => {
            state.msgHandlers[key] = func;
        },
        Del_Handler: (state, key) => {
            delete state.msgHandlers[key];
        }
    },
    getters: {},
    actions: {
        getClient({
            commit,
            rootState
        }) {
            return new Promise((resolve, reject) => {
                if (mqttclient.state.mqclient != null) {
                    resolve(mqttclient.state.mqclient);
                    return;
                }
                let relUrl = "";
                getWebIp(window.location.protocol === 'https:').then(async(rsp) => {
                    if (window.location.protocol === 'https:') {
                        relUrl = "wss://" + rsp.data + "/mqtt";
                    } else {
                        relUrl = "ws://" + rsp.data + "/mqtt";
                    }

                    let tmprss = await getClientId();
                    let options = {
                        username: rootState.user.uid,
                        password: "",
                        clientId: tmprss.data,
                        clean: true,
                        cbReconnect: async(c, next) => {
                            try {
                                let clirsp = await getClientId();
                                c.options.clientId = clirsp.data;
                                next()
                            } catch (error) {
                                if (mqttclient.state.mqclient) {
                                    // console.log(mqttclient.state.mqclient._events, 'mqttclient.state.mqclient');
                                    // mqttclient.state.mqclient._events.disconnect()
                                    if (error.code && error.code == 400 || error.code && error.code == 401 || error.code && error.code == 50012) {
                                        // console.log("进到这里",e.code);
                                        mqttclient.state.mqclient.end()
                                    }
                                    // mqttclient.state.mqclient._events.disconnect()
                                }
                            }

                        }
                    };

                    let xclient = mqtt.connect(relUrl, options);
                    xclient.on("connect", (frame) => {
                        resolve(xclient);
                    });

                    xclient.__proto__._reconnect = function() {
                        const next = () => {
                            xclient.emit('reconnect')
                            xclient._setupStream();
                        }
                        if (xclient.options.cbReconnect) {
                            xclient.options.cbReconnect(xclient, next)
                        } else {
                            next()
                        }
                    }
                    xclient.on("message", (topic, message) => {
                        if (mqttclient.state.msgHandlers.hasOwnProperty(topic)) {
                            mqttclient.state.msgHandlers[topic](message);
                        }
                    }).on('end', function() {
                        commit('CHANGE_CLIENT', null);
                    });
                    commit('CHANGE_CLIENT', xclient);
                }).catch(error => {
                    console.info(error)
                    // console.log(mqttclient.state.mqclient, 'mqttclient.state.mqclient');
                    if (mqttclient.state.mqclient && error.code && error.code == 400 || error.code && error.code == 401 || error.code && error.code == 50012) {
                        mqttclient.state.mqclient.end()
                            // mqttclient.state.mqclient._events.disconnect()
                        reject(error)
                    }
                    reject(error)
                });

            })
        }



    }
}

export default mqttclient