// 统一的服务调用工厂
import { DeviceList } from "@/api/rules/device";
import { rulesList } from "@/api/rules/ruselSevic";
import { devPlaneList } from "@/api/after/devplane";
import { deviceRoomList } from "@/api/after/room";

// 服务映射表
const services = {
  'device-select': (params) => DeviceList({ ...params, HasDeviceId: true, pageNum: 1, pageSize: 30 }),
  'rules-select': (params) => rulesList({ ...params, pageNum: 1, pageSize: 30 }),
  'plane-select': (params) => devPlaneList({ ...params, pageSize: 0 }),
  'room-select': (params) => deviceRoomList({ ...params, pageNum: 1, pageSize: 30 }),
};

// 获取服务数据
export async function getServiceData(type, params = {}) {
  if (services[type]) {
    try {
      const response = await services[type](params);
      return response.data.List || [];
    } catch (error) {
      console.error(`加载${type}数据失败`, error);
      return [];
    }
  }
  return [];
}

// 注册新服务
export function registerService(type, handler) {
  services[type] = handler;
}