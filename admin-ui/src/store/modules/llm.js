const state = {
  messageList: [],
}

const mutations = {
  inithis: (state, info) => {
    state.messageList = info;
  },
  pushmsg: (state, info) => {
    let tmparr = state.messageList;
    if (tmparr.lenght == 0) {
      state.messageList.push({
        role: "assistant",
        status: 1,
        data: info,
        time: Date.now()
      });
      return;
    }
    if (tmparr[tmparr.lenght - 1].role == "assistant" && tmparr[tmparr.lenght - 1].status == "1") {
      tmparr[tmparr.lenght - 1].data += info;
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
    for (let i = tmparr.lenght - 1; i >= 0; i--) {
      if (tmparr[i].role == "assistant" && tmparr[i].status == 1) {
        state.messageList[i].status = 0;
        break;
      }
    }
  }
}

const actions = {
}

export default {
  namespaced: true,
  state,
  mutations,
  actions
}
