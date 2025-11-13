<template>
  <div class="editor-shell">
    <QuillEditor
      ref="quillRef"
      v-model:content="content"
      content-type="html"
      theme="snow"
      :toolbar="toolbarConfig"
      :placeholder="placeholder"
    />
    <div v-if="uploading" class="editor-mask">
      <span>图片上传中...</span>
    </div>
  </div>
</template>

<script setup>
import { QuillEditor } from '@vueup/vue-quill';
import '@vueup/vue-quill/dist/vue-quill.snow.css';
import { ref, watch, computed } from 'vue';
import { ElMessage } from 'element-plus';
import http from '@/api/http.js';

const props = defineProps({
  modelValue: {
    type: String,
    default: '',
  },
  placeholder: {
    type: String,
    default: '请输入公告内容...',
  },
});
const emit = defineEmits(['update:modelValue']);

const quillRef = ref(null);
const content = ref(props.modelValue ?? '');
const uploading = ref(false);
const placeholder = computed(() => props.placeholder);

watch(
  () => props.modelValue,
  (val) => {
    if ((val ?? '') !== content.value) {
      content.value = val ?? '';
    }
  }
);
watch(content, (val) => emit('update:modelValue', val ?? ''));

const toolbarConfig = {
  container: [
    [{ header: [1, 2, 3, false] }],
    ['bold', 'italic', 'underline', 'strike'],
    [{ color: [] }, { background: [] }],
    [{ list: 'ordered' }, { list: 'bullet' }],
    [{ align: [] }],
    ['link', 'image'],
    ['clean'],
  ],
  handlers: {
    image: () => selectImage(),
  },
};

const selectImage = () => {
  const input = document.createElement('input');
  input.type = 'file';
  input.accept = 'image/*';
  input.onchange = async () => {
    const file = input.files?.[0];
    if (!file) return;
    try {
      uploading.value = true;
      const url = await uploadImage(file);
      if (url) insertImage(url);
    } catch (error) {
      console.error(error);
      ElMessage.error('图片上传失败');
    } finally {
      uploading.value = false;
    }
  };
  input.click();
};

const uploadImage = async (file) => {
  const formData = new FormData();
  formData.append('file', file);
  const res = await http.post('/api/AnnounceMents/upload', formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });
  return res?.location || res?.Location || res?.url;
};

const insertImage = (url) => {
  const quill = quillRef.value?.getQuill();
  if (!quill) return;
  const range = quill.getSelection(true) || { index: quill.getLength(), length: 0 };
  quill.insertEmbed(range.index, 'image', url, 'user');
  quill.setSelection(range.index + 1);
};
</script>

<style scoped>
.editor-shell {
  position: relative;
  width: 100%;
}

:deep(.ql-editor) {
  min-height: 420px;
  font-size: 16px;
  line-height: 1.7;
}

:deep(.ql-editor img) {
  max-width: 100%;
  height: auto;
}

.editor-mask {
  position: absolute;
  inset: 0;
  background: rgba(255, 255, 255, 0.7);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 14px;
  color: #333;
  pointer-events: none;
}
</style>
