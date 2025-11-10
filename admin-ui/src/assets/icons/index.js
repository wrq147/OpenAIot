import Vue from 'vue'

// 动态注册svg图标
let SvgIcon = () =>
    import ('@/components/SvgIcon')
Vue.component('svg-icon', SvgIcon)

const req = require.context('./svg', false, /\.svg$/)
const req2 = require.context('./hdsvg', false, /\.svg$/)
const requireAll = requireContext => requireContext.keys().map(requireContext)
requireAll(req)
requireAll(req2)