<template>
    <node :title="config.name" :show-error="showError" :content="content" :error-info="errorInfo"
        @selected="$emit('selected')" @delNode="$emit('delNode')" @insertNode="type => $emit('insertNode', type)"
        placeholder="请设置办理人" header-bgc="#18759F" header-icon="el-icon-s-check" />
</template>

<script>
import Node from './Node'

export default {
    name: "UserNode",
    props: {
        config: {
            type: Object,
            default: () => {
                return {}
            }
        }
    },
    components: { Node },
    data() {
        return {
            showError: false,
            errorInfo: '',
        }
    },
    computed: {
        content() {
            const config = this.config.props
            switch (config.assignedType) {
                case "ASSIGN_USER":
                    if (config.assignedUser.length > 0) {
                        let texts = []
                        config.assignedUser.forEach(org => texts.push(org.name))
                        return String(texts).replaceAll(',', '、')
                    } else {
                        return '请指定办理人'
                    }
                case "SELF":
                    return '发起人自己'
                case "FORM_USER":
                    if (!config.formUser || config.formUser === '') {
                        return '表单内联系人（未选择）'
                    } else {
                        let text = this.getFormItemById(config.formUser)
                        if (text && text.title) {
                            return `（${text.title}）内的人员`
                        } else {
                            return '该表单已被移除😥'
                        }
                    }
                case "ROLE":
                    if (config.role.length > 0) {
                        return String(config.role.map(x => x.name)).replaceAll(',', '、')
                    } else {
                        return '指定角色（未设置）'
                    }
                case "EQUIP_USER":
                    if (!config.formDevice || config.formDevice === '') {
                        return '表单内设备（未选择）'
                    } else {
                        console.info(config.formDevice)

                        let text = this.getFormItemById(config.formDevice)
                        if (text && text.title) {
                            return `（${text.title}）内的责任人`
                        } else {
                            return '该表单已被移除😥'
                        }
                    }
                default: return '未知设置项😥'
            }
        }
    },
    methods: {
        getFormItemById(id) {
            return this.$store.state.flowable.design.formItems.find(item => item.id === id)
        },
        //校验数据配置的合法性
        validate(err) {
            try {
                return this.showError = !this[`validate_${this.config.props.assignedType}`](err)
            } catch (e) {
                return true;
            }
        },
        validate_ASSIGN_USER(err) {
            if (this.config.props.assignedUser.length > 0) {
                return true;
            } else {
                this.errorInfo = '请指定办理人员'
                err.push(`${this.config.name} 未指定办理人员`)
                return false
            }
        },
        validate_ROLE(err) {
            if (this.config.props.role.length <= 0) {
                this.errorInfo = '请指定负责办理的角色'
                err.push(`${this.config.name} 未指定办理角色`)
                return false
            }
            return true;
        },
        validate_SELF(err) {
            return true;
        },
        validate_FORM_USER(err) {
            if (this.config.props.formUser === '') {
                this.errorInfo = '请指定表单中的人员组件'
                err.push(`${this.config.name} 办理人为表单中人员，但未指定`)
                return false
            }
            return true;
        },
        validate_EQUIP_USER(err) {
            if (this.config.props.formDevice === '') {
                this.errorInfo = '请指定表单中的设备组件'
                err.push(`${this.config.name} 办理人为表单内设备，但未指定`)
                return false
            }
            return true;
        },
        
    }
}
</script>

<style scoped></style>