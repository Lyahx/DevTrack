"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { useForm } from "react-hook-form";
import { toast } from "sonner";
import { z } from "zod";
import { GuestGuard } from "@/components/auth/AuthGuard";
import { Field } from "@/components/common/Field";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { authApi } from "@/lib/api/auth";
import { errorMessage } from "@/lib/error";
import { useAuthStore } from "@/store/auth";

const schema = z.object({
  username: z.string().min(3, "En az 3 karakter.").max(50),
  email: z.string().email("Geçerli bir e-posta gir."),
  password: z.string().min(8, "En az 8 karakter.").max(100),
});
type Values = z.infer<typeof schema>;

export default function RegisterPage() {
  const router = useRouter();
  const setAuthenticated = useAuthStore((s) => s.setAuthenticated);
  const { register, handleSubmit, formState } = useForm<Values>({
    resolver: zodResolver(schema),
    defaultValues: { username: "", email: "", password: "" },
  });

  async function onSubmit(values: Values) {
    try {
      await authApi.register(values);
      const auth = await authApi.login({ username: values.username, password: values.password });
      setAuthenticated(auth.user, auth.token);
      toast.success("Hesap oluşturuldu.");
      router.replace("/");
    } catch (e) {
      toast.error(errorMessage(e));
    }
  }

  return (
    <GuestGuard>
      <div className="flex min-h-screen items-center justify-center bg-muted/30 px-4">
        <Card className="w-full max-w-md">
          <CardHeader>
            <CardTitle className="text-2xl">Hesap oluştur</CardTitle>
            <CardDescription>DevTrack ile başla.</CardDescription>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
              <Field label="Kullanıcı adı" required error={formState.errors.username?.message}>
                <Input autoComplete="username" autoFocus {...register("username")} />
              </Field>
              <Field label="E-posta" required error={formState.errors.email?.message}>
                <Input type="email" autoComplete="email" {...register("email")} />
              </Field>
              <Field label="Şifre" required error={formState.errors.password?.message}>
                <Input type="password" autoComplete="new-password" {...register("password")} />
              </Field>
              <Button type="submit" className="w-full" disabled={formState.isSubmitting}>
                {formState.isSubmitting ? "Hesap oluşturuluyor…" : "Hesap oluştur"}
              </Button>
            </form>
          </CardContent>
          <CardFooter className="justify-center text-sm text-muted-foreground">
            Hesabın var mı?
            <Link href="/login" className="ml-1 text-primary underline-offset-4 hover:underline">
              Giriş yap
            </Link>
          </CardFooter>
        </Card>
      </div>
    </GuestGuard>
  );
}
