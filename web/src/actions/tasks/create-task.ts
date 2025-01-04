import TaskService from '@/services/task-service';
import * as yup from 'yup';

const schema = yup.object().shape({
    title: yup.string().required().max(100),
    description: yup.string().max(1000)
})

const createTask = async (formData: FormData) => {
    const title = formData.get("title") as string;
    const description = formData.get("description") as string;

    try {
        const validatedData = await schema.validate({
            title,
            description
        })

        await TaskService.createTask({
            title: validatedData.title,
            description: validatedData.description || '',
        })
    } catch (err) {
        console.log(err)
    }

}

export default createTask;