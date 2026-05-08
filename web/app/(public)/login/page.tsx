"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import { Zap } from "lucide-react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { toast } from "sonner";
import { z } from "zod";
import { GuestGuard } from "@/components/auth/AuthGuard";
import { Field } from "@/components/common/Field";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Separator } from "@/components/ui/separator";
import { authApi } from "@/lib/api/auth";
import { devApi } from "@/lib/api/dev";
import { errorMessage } from "@/lib/error";
import { useAuthStore } from "@/store/auth";

const LAST_USERNAME_KEY = "devtrack_last_username";

const schema = z.object({
  username: z.string().min(1, "Kullanıcı adı gerekli."),
  password: z.string().min(1, "Şifre gerekli."),
});
type Values = z.infer<typeof schema>;

export default function LoginPage() {
  const router = useRouter();
  const setAuthenticated = useAuthStore((s) => s.setAuthenticated);
  const [quickLoading, setQuickLoading] = useState(false);
  const { register, handleSubmit, reset, formState } = useForm<Values>({
    resolver: zodResolver(schema),
    defaultValues: { username: "", password: "" },
  });

  useEffect(() => {
    const last = typeof window !== "undefined" ? window.localStorage.getItem(LAST_USERNAME_KEY) : null;
    if (last) {
      // eslint-disable-next-line react-hooks/set-state-in-effect
      reset({ username: last, password: "" });
    }
  }, [reset]);

  async function onSubmit(values: Values) {
    try {
      const resp = await authApi.login(values);
      setAuthenticated(resp.user, resp.token);
      window.localStorage.setItem(LAST_USERNAME_KEY, resp.user.username);
      toast.success(`Hoş geldin, ${resp.user.username}`);
      router.replace("/");
    } catch (e) {
      toast.error(errorMessage(e));
    }
  }

  async function quickLogin() {
    try {
      setQuickLoading(true);
      const resp = await devApi.quickLogin();
      setAuthenticated(resp.user, resp.token);
      window.localStorage.setItem(LAST_USERNAME_KEY, resp.user.username);
      toast.success(`Dev hesabıyla girildi (${resp.user.username})`);
      router.replace("/");
    } catch (e) {
      toast.error(errorMessage(e));
    } finally {
      setQuickLoading(false);
    }
  }

  return (
    <GuestGuard>
      <div className="flex min-h-screen items-center justify-center bg-muted/30 px-4">
        <Card className="w-full max-w-md">
          <CardHeader>
            <CardTitle className="text-2xl">DevTrack&apos;e gir</CardTitle>
            <CardDescription>Hesabınla devam et.</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <Button
              variant="outline"
              className="w-full"
              onClick={quickLogin}
              disabled={quickLoading || formState.isSubmitting}
            >
              <Zap className="h-4 w-4" />
              {quickLoading ? "Giriş yapılıyor…" : "Dev: hızlı giriş (test hesabı)"}
            </Button>
            <div className="relative">
              <Separator />
              <span className="absolute left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2 bg-card px-2 text-xs text-muted-foreground">
                veya
              </span>
            </div>
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
              <Field label="Kullanıcı adı" required error={formState.errors.username?.message}>
                <Input autoComplete="username" autoFocus {...register("username")} />
              </Field>
              <Field label="Şifre" required error={formState.errors.password?.message}>
                <Input type="password" autoComplete="current-password" {...register("password")} />
              </Field>
              <Button type="submit" className="w-full" disabled={formState.isSubmitting || quickLoading}>
                {formState.isSubmitting ? "Giriş yapılıyor…" : "Giriş yap"}
              </Button>
            </form>
          </CardContent>
          <CardFooter className="justify-center text-sm text-muted-foreground">
            Hesabın yok mu?
            <Link href="/register" className="ml-1 text-primary underline-offset-4 hover:underline">
              Kayıt ol
            </Link>
          </CardFooter>
        </Card>
      </div>
    </GuestGuard>
  );
}
