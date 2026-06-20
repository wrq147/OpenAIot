import request from '@/utils/request'

// 最近会话记录
export function recentHistory() {
    return request({
        url: '/LLMService/Chat/RecentHistory',
        method: 'get'
    })
}

// 用户输入提问
export function postMessage(input, think) {
    console.info(input)
    return request({
        url: '/LLMService/Chat/Message',
        method: 'post',
        data: { "userInput": input, "thinkMode": think }
    })
}
