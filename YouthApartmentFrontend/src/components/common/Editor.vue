<template>
    <div class="tinymce-wrapper">
        <editor v-model="content"
                tag-name="div"
                :init="init"
                tinymce-script-src="https://cdn.tiny.cloud/1/49jclmud92t9rmed8od8e02i9pditx6vurjj2eheqwwoxn3t/tinymce/8/tinymce.min.js" />
    </div>
</template>
<script setup>
import Editor from "@tinymce/tinymce-vue";
import { ref, watch } from "vue";
// 后端 API 基地址（用于构建图片上传绝对 URL）
const apiBase = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5160';
// v-model
const props = defineProps({
    modelValue: String,
})
const emit = defineEmits(["update:modelValue"])
// 配置
const init = {
    language: 'zh-CN',
    language_url: '/ticymce/langs/zh-CN.js',
    menubar: false, // 隐藏菜单栏
    min_height: 700,
    max_height: 1000,
    autoresize_bottom_margin: 120,
    toolbar_mode: "sliding",
    // 精简插件，提升加载性能
    plugins:
        'wordcount searchreplace preview media image help fullscreen code table lists advlist autoresize',
    toolbar:
        "formats undo redo fontsizeselect fontselect | outdent indent aligncenter alignleft alignright | numlist bullist table removeformat | forecolor backcolor bold italic strikethrough | link preview fullscreen help",
    // 提升可读性与图片显示效果
    content_style: "body{font-size:16px;line-height:1.7;} p{margin:8px 0;} img{max-width:100%;height:auto;display:block;}",
    fontsize_formats: "12px 14px 16px 18px 24px 36px 48px 56px 72px",
    font_formats: "微软雅黑=Microsoft YaHei,Helvetica Neue,PingFang SC,sans-serif;苹果苹方= PingFang SC, Microsoft YaHei, sans- serif; 宋体 = simsun, serif; 仿宋体 = FangSong, serif; 黑体 = SimHei, sans - serif; Arial = arial, helvetica, sans - serif;Arial Black = arial black, avant garde;Book Antiqua = book antiqua, palatino; ",
    branding: false,
    elementpath: false,
    resize: false, // 禁止改变大小
    statusbar: false, // 隐藏底部状态栏
    // 图片上传到后端接口（AnnounceMentsController.Upload）
    automatic_uploads: true,
    images_upload_url: apiBase + '/api/AnnounceMents/upload',
    images_reuse_filename: false,

    // 启用 iframe 沙箱模式，增强安全性
    sandbox_iframes: true,
    // 允许同源内容，确保功能正常
    sandbox_iframes_exclusions: [window.location.host],

    // TinyMCE 许可证密钥（CDN/self-host 均支持）；若使用开源版可设为 'gpl'
    license_key: '49jclmud92t9rmed8od8e02i9pditx6vurjj2eheqwwoxn3t',
}
const content = ref(props.modelValue)
watch(props, (newVal) => content.value = newVal.modelValue)
watch(content, (newVal) => emit("update:modelValue", newVal))
</script>
<style>
.tox-tinymce-aux {
    z-index: 9999 !important;
}
/* 占满页面宽度，提升编辑区域大小 */
.tinymce-wrapper { width: 100%; }
.tinymce-wrapper .tox { min-height: 80vh; }
</style>