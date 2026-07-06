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
        data: info.isthink ? "" : info.data,
        think: info.isthink ? info.data : "",
        time: Date.now()
      });
      return;
    }

    if (tmparr[tmparr.length - 1].role == "assistant" && tmparr[tmparr.length - 1].status == "1") {
      let topres = tmparr[tmparr.length - 1];
      if (info.isthink) {
        topres.think += info.data;
      }
      else {
        topres.data += info.data;
      }
    }
    else {
      state.messageList.push({
        role: "assistant",
        status: 1,
        data: info.isthink ? "" : info.data,
        think: info.isthink ? info.data : "",
        time: Date.now()
      });
    }
  },
  finishmsg: (state, info) => {
    let tmparr = state.messageList;
    for (let i = tmparr.length - 1; i >= 0; i--) {
      if (tmparr[i].role == "assistant" && tmparr[i].status == 1) {
        if (state.messageList[i].data == "") {
          state.messageList[i].data = "会话异常，请重新尝试";
        }
        state.messageList[i].status = 0;
        break;
      }
    }
  },
  pushUserInput: (state, info) => {
    state.messageList.push({
      role: "user",
      status: 0,
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
