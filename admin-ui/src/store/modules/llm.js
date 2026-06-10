const state = {
  messageList: [],
}

const mutations = {
  inithis: (state, info) => {
    state.messageList = info;
  },
  pushmsg: (state, info) => {
    let tmparr = state.messageList;
    if (tmparr.length == 0) {
      state.messageList.push({
        role: "assistant",
        status: 1,
        data: info,
        time: Date.now()
      });
      return;
    }

    if (tmparr[tmparr.length - 1].role == "assistant" && tmparr[tmparr.length - 1].status == "1") {
      tmparr[tmparr.length - 1].data += info;
    }
    else {
      state.messageList.push({
        role: "assistant",
        status: 1,
        data: info,
        time: Date.now()
      });
    }
  },
  finishmsg: (state, info) => {
    let tmparr = state.messageList;
    for (let i = tmparr.length - 1; i >= 0; i--) {
      if (tmparr[i].role == "assistant" && tmparr[i].status == 1) {
        state.messageList[i].status = 0;
        break;
      }
    }
  },
  pushUserInput: (state, info) => {
    state.messageList.push({
      role: "user",
      status: 1,
      data: info,
      time: Date.now()
    });
  },
}

const actions = {
}

export default {
  namespaced: true,
  state,
  mutations,
  actions
}
