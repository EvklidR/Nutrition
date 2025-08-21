import { Pipe, PipeTransform } from '@angular/core';
import { PostService } from '../services/post-service/post.service';
import { firstValueFrom } from 'rxjs';

@Pipe({
  name: 'markdownImgService',
  standalone: true
})
export class MarkdownImgServicePipe implements PipeTransform {

  constructor(private postService: PostService) { }

  async transform(value: string): Promise<string> {
    if (!value) return '';

    const regex = /!\[([^\]]*)]\((\/Posts\/[^\)]+)\)/g;
    let match;
    let result = value;
    const promises = [];

    while ((match = regex.exec(value)) !== null) {
      const [full, alt, url] = match;

      const promise = firstValueFrom(this.postService.getPostImage(url))
        .then(blob => {
          const blobUrl = URL.createObjectURL(blob);
          result = result.replace(full, `<img src="${blobUrl}" alt="${alt}">`);
        })
        .catch(() => {
          result = result.replace(full, `<img alt="${alt}">`);
        });

      promises.push(promise);
    }

    await Promise.all(promises);

    return result;
  }
}
