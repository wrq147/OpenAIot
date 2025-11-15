import Cookies from 'js-cookie'

const TokenKey = 'Admin-Token'
const ShareKey = 'Share-Token';
const RefreshTokenKey = 'Admin-Refresh-Token'

export function getToken() {
  return Cookies.get(TokenKey)
}

export function getShareToken(){
  return Cookies.get(ShareKey)
}

export function setToken(token) {
  return Cookies.set(TokenKey, token)
}

export function setShareToken(token) {
  return Cookies.set(ShareKey, token)
}

export function removeToken() {
  return Cookies.remove(TokenKey)
}

export function removeShareToken() {
  return Cookies.remove(ShareKey)
}

export function getRefreshToken() {
  return Cookies.get(RefreshTokenKey)
}

export function setRefreshToken(token) {
  return Cookies.set(RefreshTokenKey, token)
}

export function removeRefreshToken() {
  return Cookies.remove(RefreshTokenKey)
}
