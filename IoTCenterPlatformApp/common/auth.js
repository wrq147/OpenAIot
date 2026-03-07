
/**令牌操作**/
const TokenKey = 'UserToken';
const RefreshTokenKey = 'UserRefreshToken';
//#ifndef H5
// 设置Token
export function setToken(value) {
	
	// sessionStorage.setItem(TokenKey,value);
	uni.setStorageSync(TokenKey, value);
}

//获取Token
export function getToken() {
	
	return uni.getStorageSync(TokenKey);
}

//删除Token
export function removeToken() {
	uni.removeStorageSync(TokenKey);
}

//获取刷新令牌
export function getRefreshToken() {
  return uni.getStorageSync(RefreshTokenKey)
}
//设置令牌
export function setRefreshToken(token) {
  uni.setStorageSync(RefreshTokenKey, token)
}
//清除令牌
export function removeRefreshToken() {
  uni.removeStorageSync(RefreshTokenKey)
}

//设置本地缓存
export function setStorageKey(key, value) {
	// sessionStorage.setItem(TokenKey,value);
	uni.setStorageSync(key, value);
}

//获取特定关键字的本地缓存数据
export function getStorageKey(key) {
	return uni.getStorageSync(key);
}

//删除特定关键字的本地缓存数据
export function removeStorageKey(key) {
	uni.removeStorageSync(key);
}
//#endif
//#ifdef H5
/**令牌操作**/
// 设置Token
export function setToken(value) {
	
	// sessionStorage.setItem(TokenKey,value);
	window.sessionStorage.setItem(TokenKey, value);
}

//获取Token
export function getToken() {
	
	return window.sessionStorage.getItem(TokenKey);
}

//删除Token
export function removeToken() {
	window.sessionStorage.removeItem(TokenKey);
}

//获取刷新令牌
export function getRefreshToken() {
  return window.sessionStorage.getItem(RefreshTokenKey)
}
//设置令牌
export function setRefreshToken(token) {
  window.sessionStorage.setItem(RefreshTokenKey, token)
}
//清除令牌
export function removeRefreshToken() {
  window.sessionStorage.removeItem(RefreshTokenKey)
}

//设置本地缓存
export function setStorageKey(key, value) {
	// sessionStorage.setItem(TokenKey,value);
	window.sessionStorage.setItem(key, value);
}

//获取特定关键字的本地缓存数据
export function getStorageKey(key) {
	return window.sessionStorage.getItem(key);
}

//删除特定关键字的本地缓存数据
export function removeStorageKey(key) {
	window.sessionStorage.removeItem(key);
}
//#endif
