/**令牌操作**/
const TokenKey = 'UserToken';
const RefreshTokenKey = 'UserRefreshToken';

export function getToken() {
  return uni.getStorageSync(TokenKey);
}

export function setToken(token) {
	uni.setStorageSync(TokenKey, token);
}

export function removeToken() {
	uni.removeStorageSync(TokenKey);
}

export function getRefreshToken() {
  return uni.getStorageSync(RefreshTokenKey)
}

export function setRefreshToken(token) {
  return uni.setStorageSync(RefreshTokenKey, token)
}

export function removeRefreshToken() {
  return uni.removeStorageSync(RefreshTokenKey)
}
